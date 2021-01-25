#include <Bela.h>
#include <cmath>
#include <iterator>
#include <iostream>
#include <vector>
#include <SampleLoader.h>
#include <SampleData.h>
#include <stdio.h>

typedef unsigned int uint;

#define NUM_CHANNELS 2

string gFilename[36];										//stores names for wav files								
SampleData gSampleData[36][NUM_CHANNELS];					

int gStartFrame = 0;										//sets a beginning from where to read the wav file
int gEndFrame = 44100 * 4.0f;								//sets the end, 44100 equals to a second, every wav file lasts for a 4 seconds
int gFrameRange;											//size of a sample
int gReadPtr;												//position of last read sample from file
int next_button = 1;										//used to cycle through buttons instead of for loop
int next_led_drum = 0;										//cycling through drum leds
int next_led_rhythm = 0;									//cycling through rhythm leds

int* button_binary_addres = new int[4];						//to store multiplexer value of a button addres
int* led_binary_addres = new int[4];						//to store multiplexer value of a led addres

bool drum_section_on = true;								//to enable scanning of the drums
bool rhythm_section_on = false;								//to enable scanning of the leds
bool vibrato_switch = true;									//to quickly change vibrato with a reset button during testing
bool vibrato_changed = false;								//to change vibrato only once while holding the button

float gInverseSampleRate;									//stores inverse samplerate	
float audio_volume = 0.1f;									//sets the maximum volume for output audio
float encoder_input;										//changes the output audio volume  
float spectra_touchpad_input;								//store analog values from the touchpads
float final_audio_output;									//stores the audio from drums and rhythm
float vibrato_timer_counter[3];								//separate counters to start the vibrato 
float button_timer_counter;									//sets how many times per second looks for input from buttons
float led_timer_counter;									//sets how often it flashes LED
float vibrato_switch_timer;									//sets how long the reset button needs to be activated to enable the vibrato
float frequency_buffer[3];									//stores a frequency from previous cycle

float c3_frequency = 130.8128;
float e3_frequency = 164.8138;
float g3_frequency = 195.9977;
float frequency_multiplier = pow(2.0, (1.0 / 12.0));		//value that if multiplies a frequency that climbs the c major scale 

struct Waveform_t {
	float audio_output;										
	float frequency;
	float phase;
	float vibrato_freq;										//value that is added to the "frequency" to simulate vibrato
	bool b_vib;												//for vibrato
};

struct Button_t {
	uint pressed = 1;										//received input from the button
	bool m_drumset = false;									//to toggle the buttons
	bool m_drumset_prev = true;								//to prevent continues switching states of the previous bool while the button is pressed
};

Button_t d_buttons[4];										//initializing array of drum buttons
Button_t r_buttons[8];										//initializing array of rhythm buttons
float spectra[3];											//initializing array for touchpads
Waveform_t spectraWave[3][4];								//initializing sound waves and their harmonics 



//loops throug an array of buttons if any of them are active(pressed) it turns off other buttons and toggles the active one 
void toggleButton(Button_t *buttons, int array_size) {
	for (int i = 0; i < array_size; i++) {
		if (buttons[i].m_drumset != buttons[i].m_drumset_prev && !buttons[i].pressed) {
			for (int j = 0; j < array_size; j++) {								//reset starts here
				if (buttons[j].m_drumset && i != j) {
					buttons[j].m_drumset = !buttons[j].m_drumset;
					buttons[j].m_drumset_prev = !buttons[j].m_drumset;
				}
			}
			buttons[i].m_drumset = !buttons[i].m_drumset;						//toggling here
			buttons[i].m_drumset_prev = buttons[i].m_drumset;
		}
		else if (buttons[i].pressed) {
			buttons[i].m_drumset_prev = !buttons[i].m_drumset;					//until the button is released m_drumset_prev needs to stay opposite of the m_drumset
		}																		//so it does not toggle the action again
	}
}

//genarates sinewave by calculating the soundwaves phase 
void generateSine(Waveform_t &wave) {
	wave.phase += 2.0f * (float)M_PI * wave.frequency * gInverseSampleRate;
	if (wave.phase > 2 * M_PI)			//by reaching the full period we want to reset it back to beginning
		wave.phase = 0;
}

//converts integers into binaries counterparts 
int* binaryConverter(int number) {
	int* ret = new int[4];
	switch (number) {
	case 1:
		ret[0] = 1; ret[1] = 0; ret[2] = 0; ret[3] = 0;
		break;
	case 2:
		ret[0] = 0; ret[1] = 1; ret[2] = 0; ret[3] = 0;
		break;
	case 3:
		ret[0] = 1; ret[1] = 1; ret[2] = 0; ret[3] = 0;
		break;
	case 4:
		ret[0] = 0; ret[1] = 0; ret[2] = 1; ret[3] = 0;
		break;
	case 5:
		ret[0] = 1; ret[1] = 0; ret[2] = 1; ret[3] = 0;
		break;
	case 6:
		ret[0] = 0; ret[1] = 1; ret[2] = 1; ret[3] = 0;
		break;
	case 7:
		ret[0] = 1; ret[1] = 1; ret[2] = 1; ret[3] = 0;
		break;
	case 8:
		ret[0] = 0; ret[1] = 0; ret[2] = 0; ret[3] = 1;
		break;
	case 9:
		ret[0] = 1; ret[1] = 0; ret[2] = 0; ret[3] = 1;
		break;
	case 10:
		ret[0] = 0; ret[1] = 1; ret[2] = 0; ret[3] = 1;
		break;
	case 11:
		ret[0] = 1; ret[1] = 1; ret[2] = 0; ret[3] = 1;
		break;
	case 12:
		ret[0] = 0; ret[1] = 0; ret[2] = 1; ret[3] = 1;
		break;
	}
	return ret;
}

//returns a frequency based on the analog value from the touchpads(position)
//we have multiple of them as a quicksolution to a fact that the touchpads need different ranges 
float SpectraFreq1(float position, float default_frequency) {
	float freq;
	if (position < 0.41)
		freq = 0;
	if (position >= 0.41 && position < 0.459)
		freq = default_frequency; 											//1
	if (position >= 0.459 * 1 && position < 0.47)
		freq = default_frequency * pow(frequency_multiplier, 2); 			//2
	if (position >= 0.47 && position < 0.49)
		freq = default_frequency * pow(frequency_multiplier, 4); 			//3
	if (position >= 0.49 && position < 0.50)
		freq = default_frequency * pow(frequency_multiplier, 5); 			//4
	if (position >= 0.50 && position < 0.52)
		freq = default_frequency * pow(frequency_multiplier, 7); 			//5
	if (position >= 0.52 && position < 0.54)
		freq = default_frequency * pow(frequency_multiplier, 9);		 	//6
	if (position >= 0.54 && position < 0.56)
		freq = default_frequency * pow(frequency_multiplier, 11);			//7
	if (position >= 0.56 && position < 0.59)
		freq = default_frequency * pow(frequency_multiplier, 12); 			//8
	if (position >= 0.59 && position < 0.62)
		freq = default_frequency * pow(frequency_multiplier, 14); 			//9
	if (position >= 0.62 && position < 0.64)
		freq = default_frequency * pow(frequency_multiplier, 16); 			//10
	if (position >= 0.64 && position < 0.67)
		freq = default_frequency * pow(frequency_multiplier, 17); 			//11
	if (position >= 0.67 && position < 0.71)
		freq = default_frequency * pow(frequency_multiplier, 19); 			//12
	if (position >= 0.71 && position < 0.75)
		freq = default_frequency * pow(frequency_multiplier, 21); 			//13
	if (position >= 0.75 && position < 0.79)
		freq = default_frequency * pow(frequency_multiplier, 23); 			//14
	if (position >= 0.79 && position < 0.83)
		freq = default_frequency * pow(frequency_multiplier, 23); 			//15
	if (position >= 0.83)
		freq = 0;
	return freq;
}

float SpectraFreq2(float position, float default_frequency) {
	float freq;
	if (position < 0.41)
		freq = 0;
	if (position >= 0.41 && position < 0.43)
		freq = default_frequency; 											//1
	if (position >= 0.43 * 1 && position < 0.445)
		freq = default_frequency * pow(frequency_multiplier, 1); 			//2
	if (position >= 0.445 && position < 0.46)
		freq = default_frequency * pow(frequency_multiplier, 3); 			//3
	if (position >= 0.46 && position < 0.482)
		freq = default_frequency * pow(frequency_multiplier, 5); 			//4
	if (position >= 0.482 && position < 0.50)
		freq = default_frequency * pow(frequency_multiplier, 7); 			//5
	if (position >= 0.50 && position < 0.525)
		freq = default_frequency * pow(frequency_multiplier, 8);		 	//6
	if (position >= 0.525 && position < 0.544)
		freq = default_frequency * pow(frequency_multiplier, 10);			//7
	if (position >= 0.544 && position < 0.572)
		freq = default_frequency * pow(frequency_multiplier, 12); 			//8
	if (position >= 0.572 && position < 0.60)
		freq = default_frequency * pow(frequency_multiplier, 13); 			//9
	if (position >= 0.60 && position < 0.634)
		freq = default_frequency * pow(frequency_multiplier, 15); 			//10
	if (position >= 0.634 && position < 0.666)
		freq = default_frequency * pow(frequency_multiplier, 17); 			//11
	if (position >= 0.666 && position < 0.72)
		freq = default_frequency * pow(frequency_multiplier, 19); 			//12
	if (position >= 0.72 && position < 0.75)
		freq = default_frequency * pow(frequency_multiplier, 20); 			//13
	if (position >= 0.75 && position < 0.79)
		freq = default_frequency * pow(frequency_multiplier, 22); 			//14
	if (position >= 0.79 && position < 0.83)
		freq = default_frequency * pow(frequency_multiplier, 22); 			//15
	if (position >= 0.83)
		freq = 0;
	return freq;
}

float SpectraFreq3(float position, float default_frequency) {
	float freq;
	if (position < 0.41)
		freq = 0;
	if (position >= 0.41 && position < 0.459)
		freq = default_frequency; 											//1
	if (position >= 0.459 * 1 && position < 0.47)
		freq = default_frequency * pow(frequency_multiplier, 2); 			//2
	if (position >= 0.47 && position < 0.49)
		freq = default_frequency * pow(frequency_multiplier, 4); 			//3
	if (position >= 0.49 && position < 0.50)
		freq = default_frequency * pow(frequency_multiplier, 5); 			//4
	if (position >= 0.50 && position < 0.52)
		freq = default_frequency * pow(frequency_multiplier, 7); 			//5
	if (position >= 0.52 && position < 0.54)
		freq = default_frequency * pow(frequency_multiplier, 9);		 	//6
	if (position >= 0.54 && position < 0.56)
		freq = default_frequency * pow(frequency_multiplier, 10);			//7
	if (position >= 0.56 && position < 0.59)
		freq = default_frequency * pow(frequency_multiplier, 12); 			//8
	if (position >= 0.59 && position < 0.62)
		freq = default_frequency * pow(frequency_multiplier, 14); 			//9
	if (position >= 0.62 && position < 0.64)
		freq = default_frequency * pow(frequency_multiplier, 16); 			//10
	if (position >= 0.64 && position < 0.67)
		freq = default_frequency * pow(frequency_multiplier, 17); 			//11
	if (position >= 0.67 && position < 0.71)
		freq = default_frequency * pow(frequency_multiplier, 19); 			//12
	if (position >= 0.71 && position < 0.75)
		freq = default_frequency * pow(frequency_multiplier, 21); 			//13
	if (position >= 0.75 && position < 0.79)
		freq = default_frequency * pow(frequency_multiplier, 22); 			//14
	if (position >= 0.79 && position < 0.83)
		freq = default_frequency * pow(frequency_multiplier, 22); 			//15
	if (position >= 0.83)
		freq = 0;
	return freq;
}

//simulates the vibrato effect
float modVibrato(Waveform_t &wave, float vibrato, float depth) {
	if (wave.vibrato_freq >= depth)
		wave.b_vib = true;
	if (wave.vibrato_freq <= 0)
		wave.b_vib = false;

	vibrato *= (gInverseSampleRate * 2);				//timing of the vibrato so it will be twice as fast as a second, making the previous two statements happen twice 
														//this way it follows a linear increas from 0 to the depth(e.g 6.5) similarly as triangle wave 
	if (wave.b_vib) {
		wave.vibrato_freq -= abs(depth * vibrato);		//when it reaches the depth, this scope starts decreasing the vibrato frequency
		return wave.vibrato_freq;
	}
	else {
		wave.vibrato_freq += abs(depth * vibrato);
		return wave.vibrato_freq;
	}
}

bool setup(BelaContext *context, void *userData)
{
	//initializing the names for the 36 audio files
	for (int i = 0; i < 36; i++) {
		if (i < 4)
			gFilename[i] = string(1, ('A' + i)) + "D" + to_string(1) + ".wav";
		if (i >= 4 && i < 12)
			gFilename[i] = "AG" + to_string(i - 3) + ".wav";
		if (i >= 12 && i < 20)
			gFilename[i] = "BG" + to_string(i - 11) + ".wav";
		if (i >= 20 && i < 28)
			gFilename[i] = "CG" + to_string(i - 19) + ".wav";
		if (i >= 28 && i < 36)
			gFilename[i] = "DG" + to_string(i - 27) + ".wav";
	}

	// getNumFrames() and getSamples() are helper functions for getting data from wav files declared in SampleLoader.h
	// SampleData is a struct that contains an array of floats and an int declared in SampleData.h

	gFrameRange = gEndFrame - gStartFrame;
	for (int ch = 0; ch < NUM_CHANNELS; ch++) {
		for (int i = 0; i < 36; i++) {
			gSampleData[i][ch].sampleLen = getNumFrames(gFilename[i]);
			gSampleData[i][ch].samples = new float[gFrameRange];
			getSamples(gFilename[i], gSampleData[i][ch].samples, ch, gStartFrame, gEndFrame);
		}
	}

	gInverseSampleRate = 1.0f / context->audioSampleRate;

	//initializing the pins on bela 
	//LED multiplexer 
	pinMode(context, 0, P8_18, INPUT);		//SIG  
	pinMode(context, 0, P8_27, OUTPUT);		//s3
	pinMode(context, 0, P8_28, OUTPUT);		//s2
	pinMode(context, 0, P8_29, OUTPUT);		//s1
	pinMode(context, 0, P8_30, OUTPUT);		//s0

	//buttons multiplexer
	pinMode(context, 0, P8_08, OUTPUT);		//SIG
	pinMode(context, 0, P8_16, OUTPUT);		//s3
	pinMode(context, 0, P8_10, OUTPUT);		//s2
	pinMode(context, 0, P8_11, OUTPUT);		//s1
	pinMode(context, 0, P8_12, OUTPUT);		//s0

	pinMode(context, 0, P9_12, INPUT);		//reset button

	return true;
}


void render(BelaContext *context, void *userData)
{
	for (unsigned int n = 0; n < context->audioFrames; n++) {

		//scanning for reset button
		if (digitalRead(context, 0, P9_12) == 0) {
			for (int i = 0; i < 4; i++) {			//resetting drums
				d_buttons[i].m_drumset = false;	
				d_buttons[i].m_drumset_prev = !d_buttons[i].m_drumset;
			}

			for (int i = 0; i < 8; i++) {			//resetting rhythm
				r_buttons[i].m_drumset = false;
				r_buttons[i].m_drumset_prev = !r_buttons[i].m_drumset;
			}

			vibrato_switch_timer += gInverseSampleRate;		//if reset button is hold for more than 3seconds, toggle vibrato
			if (vibrato_switch_timer >= 3.f && !vibrato_changed) {
				vibrato_switch = !vibrato_switch;
				vibrato_changed = true;
			}

		}
		else {
			vibrato_switch_timer = 0.f;						//reset the timer which counts towards the change of vibrato
			vibrato_changed = vibrato_changed = false;
		}

		button_timer_counter += gInverseSampleRate;
		if (button_timer_counter >= 0.019) {				//refresh rate for scanning the buttons


			if (!d_buttons[0].m_drumset && !d_buttons[1].m_drumset && !d_buttons[2].m_drumset && !d_buttons[3].m_drumset) {		//if any of the drums are turn off turn also all of the rhythms
				for (int i = 0; i < 8; i++) {
					r_buttons[i].m_drumset = false;
					r_buttons[i].m_drumset_prev = !r_buttons[i].m_drumset;
				}
			}


			button_binary_addres = binaryConverter(next_button);		//send data to the button
			digitalWrite(context, n, P8_30, button_binary_addres[0]);
			digitalWrite(context, n, P8_29, button_binary_addres[1]);
			digitalWrite(context, n, P8_28, button_binary_addres[2]);
			digitalWrite(context, n, P8_27, button_binary_addres[3]);

			for (int i = 0; i < 4; i++) {								//resetting the input values 
				if (d_buttons[i].pressed == 0)
					d_buttons[i].pressed = 1;
			}

			for (int i = 0; i < 8; i++) {								//resetting the input values
				if (r_buttons[i].pressed == 0)
					r_buttons[i].pressed = 1;
			}

			if (digitalRead(context, 0, P8_18) == 0) {
				switch (next_button) {
				case 1:
					d_buttons[0].pressed = 0;
					break;
				case 2:
					r_buttons[7].pressed = 0;
					break;
				case 3:
					r_buttons[6].pressed = 0;
					break;
				case 4:
					r_buttons[5].pressed = 0;
					break;
				case 5:
					r_buttons[4].pressed = 0;
					break;
				case 6:
					r_buttons[0].pressed = 0;
					break;
				case 7:
					r_buttons[1].pressed = 0;
					break;
				case 8:
					r_buttons[2].pressed = 0;
					break;
				case 9:
					r_buttons[3].pressed = 0;
					break;
				case 10:
					d_buttons[3].pressed = 0;
					break;
				case 11:
					d_buttons[2].pressed = 0;
					break;
				case 12:
					d_buttons[1].pressed = 0;
					break;
				}
			}
			else {															//resets all of the led
				digitalWrite(context, n, P8_16, led_binary_addres[0]);
				digitalWrite(context, n, P8_12, led_binary_addres[1]);
				digitalWrite(context, n, P8_11, led_binary_addres[2]);
				digitalWrite(context, n, P8_10, led_binary_addres[3]);
				digitalWrite(context, n, P8_08, 0);
			}

			next_button++;
			if (next_button == 13)
				next_button = 1;
			button_timer_counter = 0;
		}

		led_timer_counter += gInverseSampleRate;
		if (led_timer_counter >= 0.001) {						//refresh rate for turning on the leds
			if (drum_section_on) {
				if (d_buttons[next_led_drum].m_drumset && next_led_drum != 5) {
					switch (next_led_drum) {
					case 0:
						led_binary_addres = binaryConverter(12);
						break;
					case 1:
						led_binary_addres = binaryConverter(11);
						break;
					case 2:
						led_binary_addres = binaryConverter(10);
						break;
					case 3:
						led_binary_addres = binaryConverter(9);
						break;
					}	//send data to multiplexer
					digitalWrite(context, n, P8_16, led_binary_addres[0]);
					digitalWrite(context, n, P8_12, led_binary_addres[1]);
					digitalWrite(context, n, P8_11, led_binary_addres[2]);
					digitalWrite(context, n, P8_10, led_binary_addres[3]);
					digitalWrite(context, n, P8_08, 1);
				}
				next_led_drum++;
				if (next_led_drum == 5) {
					drum_section_on = false;
					rhythm_section_on = true;
					next_led_drum = 0;
				}
			}

			if (rhythm_section_on) {
				if (r_buttons[next_led_rhythm].m_drumset) {
					switch (next_led_rhythm) {
					case 0:
						led_binary_addres = binaryConverter(5);
						break;
					case 1:
						led_binary_addres = binaryConverter(6);
						break;
					case 2:
						led_binary_addres = binaryConverter(7);
						break;
					case 3:
						led_binary_addres = binaryConverter(8);
						break;
					case 4:
						led_binary_addres = binaryConverter(4);
						break;
					case 5:
						led_binary_addres = binaryConverter(3);
						break;
					case 6:
						led_binary_addres = binaryConverter(2);
						break;
					case 7:
						led_binary_addres = binaryConverter(1);
						break;
					}	//send data to multiplexer
					digitalWrite(context, n, P8_16, led_binary_addres[0]);
					digitalWrite(context, n, P8_12, led_binary_addres[1]);
					digitalWrite(context, n, P8_11, led_binary_addres[2]);
					digitalWrite(context, n, P8_10, led_binary_addres[3]);
					digitalWrite(context, n, P8_08, 1);
				}
				next_led_rhythm++;
				if (next_led_rhythm == 8) {
					next_led_rhythm = 0;
					rhythm_section_on = false;
					drum_section_on = true;
				}
			}
			led_timer_counter = 0;
		}

		//receiving data from analog channels
		encoder_input = analogRead(context, n, 3);								
		spectra[0] = analogRead(context, n, 0);
		spectra[1] = analogRead(context, n, 1);
		spectra[2] = analogRead(context, n, 2);

		//based on the analog input we assigned proper frequency from preselected range
		spectraWave[0][0].frequency = SpectraFreq1(spectra[0], c3_frequency);
		spectraWave[1][0].frequency = SpectraFreq2(spectra[1], e3_frequency);
		spectraWave[2][0].frequency = SpectraFreq3(spectra[2], g3_frequency);

		//saving the current frequency to the buffer if there is a difference
		for (int i = 0; i < 3; i++) {
			if (spectraWave[i][0].frequency != frequency_buffer[i]) {
				vibrato_timer_counter[i] = 0;
				frequency_buffer[i] = spectraWave[i][0].frequency;
			}
		}

		//applying vibrato to the frequency
		for (int wava = 0; wava < 3; wava++) {

			if (spectra[wava] > 0.1 && vibrato_switch) {
				if (vibrato_timer_counter[wava] >= 0.5) {		//vibrato active is here limited to 0.5 seconds
					spectraWave[wava][0].frequency += modVibrato(spectraWave[wava][0], 4.0f, 6.5f*(spectraWave[wava][0].frequency / 125));	//specifying the vibrato parameters
				}																															//repeating 4 times a second with 6.5 frequency
				vibrato_timer_counter[wava] += gInverseSampleRate;																			//multiplied by the actual frequency dividied by a value
			}																																//to get a dynamic range for high frequencies
			else {
				spectraWave[wava][0].vibrato_freq = 0;
				vibrato_timer_counter[wava] = 0;
			}
			//adding harmonics to the original sinewave
			//the vibrato is not applied to the harmonics and they are turned down by in amplitude
			generateSine(spectraWave[wava][0]);
			spectraWave[wava][0].audio_output = (0.4f) * sinf(spectraWave[wava][0].phase);
			spectraWave[wava][1].frequency = frequency_buffer[wava] * 2;
			generateSine(spectraWave[wava][1]);
			spectraWave[wava][0].audio_output += (0.004f) * sinf(spectraWave[wava][1].phase);
			spectraWave[wava][2].frequency = spectraWave[wava][1].frequency * 2;
			generateSine(spectraWave[wava][2]);
			spectraWave[wava][0].audio_output += (0.0001f) * sinf(spectraWave[wava][2].phase);
			spectraWave[wava][3].frequency = spectraWave[wava][2].frequency * 2;
			generateSine(spectraWave[wava][3]);
			spectraWave[wava][0].audio_output += (0.00007f) * sinf(spectraWave[wava][3].phase);
			spectraWave[wava][0].audio_output = (audio_volume*0.8f) * spectraWave[wava][0].audio_output;
		}

		if (encoder_input > 0.1f)
			audio_volume = encoder_input * 1.f;


		if (++gReadPtr > gEndFrame)
			gReadPtr = 0;

		final_audio_output = 0;
		for (unsigned int channel = 0; channel < context->audioOutChannels; channel++) {
			// Wrap channel index in case there are more audio output channels than the file contains

			//grabbing all of the audio and outputing it to the final output
			for (int i = 0; i < 4; i++) {
				if (d_buttons[i].m_drumset) {
					final_audio_output += gSampleData[i][channel%NUM_CHANNELS].samples[gReadPtr] * audio_volume;
					for (int j = 0; j < 8; j++) {
						if (r_buttons[j].m_drumset) {
							final_audio_output += gSampleData[4 + j + (i * 8)][channel%NUM_CHANNELS].samples[gReadPtr] * audio_volume;
						}
					}
				}
			}
			audioWrite(context, n, channel, final_audio_output + spectraWave[0][0].audio_output + spectraWave[1][0].audio_output + spectraWave[2][0].audio_output);
		}

		toggleButton(d_buttons, sizeof(d_buttons) / sizeof(Button_t));
		toggleButton(r_buttons, sizeof(r_buttons) / sizeof(Button_t));

	}
}


void cleanup(BelaContext *context, void *userData)
{}