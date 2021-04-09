package com.group5.fap;

import android.app.Activity;
import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.ProgressBar;
import android.widget.TextView;

import java.util.ArrayList;
import java.util.List;

public class ListAdapter extends ArrayAdapter<ListViewElements> {
    private final Context context;
    private int layoutResourceID;
    private ArrayList<ListViewElements> objects;


    public ListAdapter(Context context, int layoutResourceID, ArrayList<ListViewElements> objects) {
        super(context, layoutResourceID, objects);

        this.context = context;
        this.layoutResourceID = layoutResourceID;
        this.objects = objects;
    }


    @Override
    public View getView(int position, final View convertView, ViewGroup parent) {

        View row = convertView;

        if (row == null) {
            LayoutInflater inflater = ((Activity) context).getLayoutInflater();
            row = inflater.inflate(layoutResourceID, parent, false);
        }
        ProgressBar itemProgressBar = (ProgressBar) row.findViewById(R.id.progressBar);
        TextView itemTextView = (TextView) row.findViewById(R.id.textView);
        TextView timeView = (TextView) row.findViewById(R.id.timeView);



        final ListViewElements list = objects.get(position);                    // get your data object
        itemProgressBar.setMax(10000);                                  // assign values to your items of listView
        itemProgressBar.setProgress((int)list.getProgressBarValue());
        itemTextView.setText(list.getTextViewString());
        timeView.setText(list.getTimeView());

        return row;
    }

}
