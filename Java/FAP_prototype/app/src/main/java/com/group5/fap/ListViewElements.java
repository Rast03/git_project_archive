package com.group5.fap;

public class ListViewElements {
    private double progressBarValue;
    private String textViewString;
    private String timeView;

    public ListViewElements(double progressBarValue, String textViewString){
        this.progressBarValue = progressBarValue;
        this.textViewString = textViewString;
        this.timeView = " ";
    }

    public double getProgressBarValue() {
        return progressBarValue;
    }

    public String getTextViewString() {
        return textViewString;
    }

    public String getTimeView() { return timeView; }

    public void setProgressBarValue(double progressBarValue) { this.progressBarValue = progressBarValue; }

    public void setTextViewString(String textViewString) { this.textViewString = textViewString; }

    public void setTimeView(String timeView) { this.timeView = timeView; }
}
