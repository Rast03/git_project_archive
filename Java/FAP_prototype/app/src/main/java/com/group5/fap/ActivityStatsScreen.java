package com.group5.fap;

import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.support.v7.widget.Toolbar;
import android.view.View;
import android.widget.ListView;


public class ActivityStatsScreen extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_stats_screen);

        Toolbar toolbar = findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);

        UsageStatsTranslator usageStatsTranslator = new UsageStatsTranslator(this);

        ListAdapter listAdapter = new ListAdapter(this, R.layout.list_view_elements, usageStatsTranslator.getDetailedUsageStats(usageStatsTranslator.getTranslatedList()));
        ListView listView = findViewById(R.id.listView2);
        listView.setAdapter(listAdapter);

    }


    public void onClick(View view) {
        startActivity(new Intent(view.getContext(), HomeScreen.class));
    }
}
