package com.group5.fap;

import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ListView;

import java.util.ArrayList;
import java.util.concurrent.ThreadLocalRandom;

public class ActivityResultsScreen extends AppCompatActivity {

    private ArrayList<ListViewElements> listViewElements = new ArrayList<>();

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_results_screen);

        listViewElements.add(new ListViewElements(ThreadLocalRandom.current().nextInt(4000,7000),"Amager beach"));
        listViewElements.add(new ListViewElements(ThreadLocalRandom.current().nextInt(2600,5000),"Copenhagen zoo"));
        listViewElements.add(new ListViewElements(ThreadLocalRandom.current().nextInt(2600,6000),"Experimentarium"));
        listViewElements.add(new ListViewElements(ThreadLocalRandom.current().nextInt(5000,7000),"Fishing"));
        listViewElements.add(new ListViewElements(ThreadLocalRandom.current().nextInt(5000,7000),"Kings garden"));
        listViewElements.add(new ListViewElements(ThreadLocalRandom.current().nextInt(5000,7000),"Rosenborg castle"));
        listViewElements.add(new ListViewElements(ThreadLocalRandom.current().nextInt(5000,7000),"Rundetårn"));
        listViewElements.add(new ListViewElements(ThreadLocalRandom.current().nextInt(3500,4000),"The Blue Planet"));
        listViewElements.add(new ListViewElements(ThreadLocalRandom.current().nextInt(2600,5000),"The Incredibles 2"));
        listViewElements.add(new ListViewElements(ThreadLocalRandom.current().nextInt(2600,4000),"Tivoli"));

        ListAdapter listAdapter = new ListAdapter(this, R.layout.list_view_elements, listViewElements);
        ListView listView = findViewById(R.id.listView);
        listView.setAdapter(listAdapter);



    }

    @Override
    public void onBackPressed() {
        startActivity(new Intent(this,HomeScreen.class));
    }

    public void onClick(View view) {
        startActivity(new Intent(view.getContext(), HomeScreen.class));
    }
}