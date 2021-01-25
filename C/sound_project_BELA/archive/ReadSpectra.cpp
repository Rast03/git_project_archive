/***** ReadSpectra.cpp *****/

#include <Bela.h>
#include <stdio.h>
#include <cmath>
#include <Scope.h>

typedef unsigned int uint;
//----------------------

// set the frequency of the oscillators
float gInverseSampleRate;
Scope scope;
float _audio_volume = 0.1f;
float _spectra;
float BPM;
float timing;
float _encoder;
int i;
float drum_one[16] = { 4,0,0,4, 0,4,0,4, 4,0,0,4, 0,4,4,0 };
float snare_one[16] = { 0,0,4,0, 0,0,4,0, 0,0,4,0, 0,0,0,0 };

struct Waveform_t {
	float out;
	float frequency;
	float phase;
	bool enable;
	char part = 'A';
	int sustainTimer;
	float ADSR;
	float ADSRlength;
	bool b_vib;
};

struct Drum_button_t {
	uint pressed;
	bool m_drumset = false;
	bool m_drumset_prev = true;
};

struct Rythm_button_t {
	uint pressed;
	bool m_drumset = false;
	bool m_drumset_prev = true;
};


//Assign drum & rythm buttons to arrays instead
Drum_button_t d_buttons[4];
Rythm_button_t r_buttons[8];

Waveform_t sinewave;
Waveform_t staticwave;
Waveform_t trianglewave;
Waveform_t trianglewave_octave;
Waveform_t base_drum[16];
Waveform_t snare_drum[5];

void toggleButton(Drum_button_t &button1) {

	if (button1.m_drumset != button1.m_drumset_prev && button1.pressed) {
		button1.m_drumset = !button1.m_drumset;
		button1.m_drumset_prev = button1.m_drumset;
		rt_printf("prev %d, pressed %d", button1.m_drumset_prev, button1.m_drumset);
	}
	else {
		button1.m_drumset_prev = !button1.m_drumset;
	}
}

void configureADSR(Waveform_t &wave, float attackPoint, float decayPoint, float attackLength, float decayLength, float sustainLength, float releaseLength, float sampleLength) {
	switch (wave.part) {

	case 'A':
		wave.ADSR += attackPoint / ((1 / (sampleLength*gInverseSampleRate))*attackLength);
		if (wave.ADSR >= attackPoint) {
			wave.ADSR = attackPoint;
			wave.part = 'D';
		}
		break;
	case 'D':
		wave.ADSR -= (1 - decayPoint) / ((1 / (sampleLength*gInverseSampleRate))*decayLength);
		if (wave.ADSR <= decayPoint) {
			wave.ADSR = decayPoint;
			wave.part = 'S';
		}
		break;
	case 'S':
		wave.sustainTimer += 1;
		if (wave.sustainTimer >= (1 / (sampleLength*gInverseSampleRate))*sustainLength) {
			wave.part = 'R';
			wave.sustainTimer = 0;
		}
		break;
	case 'R':
		wave.ADSR -= decayPoint / ((1 / (sampleLength*gInverseSampleRate))*releaseLength);
		if (wave.ADSR <= 0) {
			wave.ADSR = 0;
			wave.part = 'A';
		}
		break;
	}
}

void modVibrato(Waveform_t &wave, float origfrequency, float vibrato) {
	if (wave.frequency >= origfrequency + 50.f)
		wave.b_vib = true;
	if (wave.frequency <= origfrequency)
		wave.b_vib = false;

	vibrato *= gInverseSampleRate;

	if (wave.b_vib) {
		wave.frequency -= abs(wave.frequency * vibrato);
	}
	else {
		wave.frequency += abs(wave.frequency * vibrato);
	}
}

void generateTriangle(Waveform_t &wave) {
	if (wave.phase > (float)M_PI && !wave.enable)
		wave.enable = true;
	if (wave.phase < (float)M_PI*(-1) && wave.enable)
		wave.enable = false;
	if (wave.enable)
		wave.phase -= 2.0f * (float)M_PI * wave.frequency * gInverseSampleRate;
	if (!wave.enable)
		wave.phase += 2.0f * (float)M_PI * wave.frequency * gInverseSampleRate;
}


void generateSine(Waveform_t &wave) {
	wave.phase += 2.0f * (float)M_PI * wave.frequency * gInverseSampleRate;
	if (wave.phase > 2 * M_PI)
		wave.phase -= 2.0f * (float)M_PI;
}


//return the frequency, arguments being float position of the touchpad
float SpectraFreq(float position) {
	float freq;
	if (position <= 0.125)
		freq = 130.81; //then frequency 130.81
	if (position > 0.126 && position < 0.25)
		freq = 146.83;// then frequency 146.83
	if (position > 0.26 && position < 0.375)
		freq = 164.81; // then frequency 164.81
	if (position > 0.376 && position < 0.5)
		freq = 174.61; // frequency 174.61
	if (position > 0.51 && position < 0.625)
		freq = 185;// frequency 185 
	if (position > 0.626 && position < 0.75)
		freq = 196; // frequency 196
	if (position > 0.76 && position < 0.875)
		freq = 220; // frequency 220
	if (position > 0.876)
		freq = 246.94; // frequency 246.94
	return freq;

}




bool setup(BelaContext *context, void *userData)
{
	// tell the scope how many channels and the sample rate
	scope.setup(4, context->audioSampleRate);
	gInverseSampleRate = 1.0f / context->audioSampleRate;

	//Determine pin_modes for digital I/O
	pinMode(context, 0, P8_07, INPUT);
	pinMode(context, 0, P8_08, INPUT);
	pinMode(context, 0, P8_09, INPUT);
	pinMode(context, 0, P8_10, INPUT);



	sinewave.frequency = 220.f;
	base_drum[0].ADSRlength = drum_one[0];
	snare_drum[0].ADSRlength = snare_one[0];
	//Return true cus we like that
	return true;
}

void render(BelaContext *context, void *userData)
{

	// iterate over the audio frames and create three oscillators, seperated in phase by PI/2
	for (unsigned int n = 0; n < context->audioFrames; ++n)
	{
		/////////////////////////////////////////////
		//DEBUG -------------------------------------
		/////////////////////////////////////////////

		d_buttons[0].m_drumset = true;
		//_spectra = 0.5f;
		trianglewave.frequency = 220.0f;
		trianglewave_octave.frequency = 220.0f * 5;
		base_drum[0].frequency = 48;
		base_drum[1].frequency = 34;
		base_drum[2].frequency = 88;

		snare_drum[0].frequency = 190;
		snare_drum[1].frequency = 220;
		snare_drum[2].frequency = 250;

		_audio_volume = 0.5;
		float out1 = 0;

		float out5 = 0;
		//ADSR Control example
		//parameters required: wave, (float)attackPoint, (float)decayPoint, (float)attackLength, (float)decayLength, (float)sustainLength, (float)releaseLength,



		/////////////////////////////////////////////
		//DEBUG -------------------------------------
		/////////////////////////////////////////////



		timing++;
		if (timing == (1 / (gInverseSampleRate * 4))) {
			i++;
			base_drum[0].ADSRlength = drum_one[i];
			snare_drum[0].ADSRlength = snare_one[i];
			if (i == 15) {
				i = 0;
				base_drum[0].ADSRlength = drum_one[i];
				snare_drum[0].ADSRlength = snare_one[i];
			}
			timing = 0;
		}
		configureADSR(base_drum[0], 1.f, 0.15f, 0.01f, 0.09f, 0.05f, 0.85f, base_drum[0].ADSRlength);
		configureADSR(snare_drum[0], 1.f, 0.05f, 0.01f, 0.05f, 0.1f, 0.84f, snare_drum[0].ADSRlength);

		sinewave.out = _audio_volume * sinf(sinewave.phase);
		staticwave.out = _audio_volume * sinf(staticwave.phase);
		trianglewave.out = _audio_volume * (trianglewave.phase);
		trianglewave_octave.out = _audio_volume * (trianglewave_octave.phase);

		base_drum[0].out = _audio_volume * sinf(base_drum[0].phase) *0.5*base_drum[0].ADSR;
		base_drum[1].out = _audio_volume * sinf(base_drum[1].phase) *0.5*base_drum[0].ADSR;
		base_drum[2].out = _audio_volume * sinf(base_drum[2].phase) *0.25*base_drum[0].ADSR;
		base_drum[3].out = _audio_volume * (base_drum[3].phase) *0.005*base_drum[0].ADSR;

		snare_drum[0].out = _audio_volume * sinf(snare_drum[0].phase) *0.6*snare_drum[0].ADSR;
		snare_drum[1].out = _audio_volume * sinf(snare_drum[1].phase) *0.8*snare_drum[0].ADSR;
		snare_drum[2].out = _audio_volume * sinf(snare_drum[2].phase) *0.7*snare_drum[0].ADSR;
		snare_drum[3].out = _audio_volume * (snare_drum[3].phase) *0.0125*snare_drum[0].ADSR;
		snare_drum[4].out = _audio_volume * (snare_drum[4].phase) *0.0225*snare_drum[0].ADSR;

		for (int i = 0; i < 4; i++) {
			out1 += base_drum[i].out;
		}

		for (int i = 0; i < 5; i++) {
			out5 += snare_drum[i].out;
		}

		// Read shit from bela <<ANALOG>>
		_spectra = analogRead(context, n, 1);
		_encoder = analogRead(context, n, 2);
		// frequency value for the first touchpad. 
		_spectra = SpectraFreq(_spectra);
		// Read shit from bela <<DIGITAL>>
		d_buttons[0].pressed = digitalRead(context, n, P8_07);
		d_buttons[1].pressed = digitalRead(context, n, P8_08);
		d_buttons[2].pressed = digitalRead(context, n, P8_09);
		d_buttons[3].pressed = digitalRead(context, n, P8_10);

		// Control amplitude / volume with encoder
		if (_encoder > 0.1f)
			_audio_volume = _encoder * 0.25f;

		generateSine(base_drum[0]);
		generateSine(base_drum[1]);
		generateSine(base_drum[2]);
		base_drum[3].phase = (rand() % 3) - 1;

		generateSine(snare_drum[0]);
		generateSine(snare_drum[1]);
		generateSine(snare_drum[2]);
		snare_drum[3].phase = (rand() % 3) - 1;
		snare_drum[4].phase = (rand() % 3) - 1;




		//Spectra touhpad controls
		if (_spectra >= 0.0001f && _spectra <= 0.98f) {
			sinewave.frequency = 264.0f * 10 * _spectra;
			rt_printf("%f", _spectra);
		}
		else {
			sinewave.frequency = 0.f;
		}

		// Controlling phases of the different signals
		generateSine(sinewave);
		modVibrato(sinewave, 220.f, 1.7f);
		generateTriangle(trianglewave);
		generateTriangle(trianglewave_octave);


		//We need a wrapper function for the button
		toggleButton(d_buttons[0]);

		// For loop to stream to both channels (or more)
		for (unsigned int channel = 0; channel < std::min(context->audioInChannels, context->audioOutChannels); channel++) {
			audioWrite(context, n, channel, out1 + out5);
		}

		//Scope the stuff in the oscillator
		// scope.log(base_drum[0].out+base_drum[1].out+base_drum[2].out+base_drum[3].out,base_drum[4].out+base_drum[5].out+base_drum[6].out+base_drum[7].out,base_drum[8].out+base_drum[9].out+base_drum[10].out+base_drum[11].out,base_drum[12].out+base_drum[13].out+base_drum[14].out+base_drum[15].out);

		scope.log(out1, out5, out1, out5);
	}
}

void cleanup(BelaContext *context, void *userData) {}