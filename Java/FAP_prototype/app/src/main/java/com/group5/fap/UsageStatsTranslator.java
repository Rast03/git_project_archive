package com.group5.fap;

import android.app.AppOpsManager;
import android.app.usage.UsageStats;
import android.app.usage.UsageStatsManager;
import android.content.Context;
import android.content.pm.PackageManager;
import android.os.Process;
import android.util.Log;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;
import java.util.concurrent.TimeUnit;

import static android.app.AppOpsManager.MODE_ALLOWED;
import static android.app.AppOpsManager.OPSTR_GET_USAGE_STATS;

public class UsageStatsTranslator {
    Context context;
    ArrayList<ListViewElements> listViewElements;
    double bigSum;

    UsageStatsTranslator(Context context){
        this.context = context;
        listViewElements = new ArrayList<>();
        bigSum = 0;
    }

    public ArrayList<ListViewElements> getTranslatedList() {

//        long startTime = new GregorianCalendar(2017, 0, 1).getTimeInMillis();
//        long endTime = new GregorianCalendar(2019, 0, 1).getTimeInMillis();
        long time = System.currentTimeMillis();


        UsageStatsManager usageStatsManager = (UsageStatsManager)context.getSystemService(Context.USAGE_STATS_SERVICE);
        List<UsageStats> queryUsageStats = usageStatsManager.queryUsageStats(UsageStatsManager.INTERVAL_BEST, time - 3 * 24 * 3600 * 1000, time);
        PackageManager pm = context.getPackageManager();

        for (int i = 0; i < queryUsageStats.size(); i++) {


            Log.d("check", queryUsageStats.get(i).getPackageName());

            try {
                if ((queryUsageStats.get(i).getTotalTimeInForeground() != 0)
                        && (pm.getInstallerPackageName(queryUsageStats.get(i).getPackageName()) != null
                        //blacklist of apps
                        && !queryUsageStats.get(i).getPackageName().equals("com.android.vending")
                        //whitelist of preinstalled apps starts here:
                        || queryUsageStats.get(i).getPackageName().equals("com.google.android.youtube")
                        || queryUsageStats.get(i).getPackageName().equals("com.android.chrome"))) {
                    String name = "error";
                    try {
                        name = pm.getApplicationInfo(queryUsageStats.get(i).getPackageName(), 0).loadLabel(pm).toString();
                    } catch (PackageManager.NameNotFoundException e) {
                        e.printStackTrace();
                    }

                    boolean check = false;
                    for (ListViewElements lVE : listViewElements) {
                        if (lVE.getTextViewString().equals(name)) {
                            lVE.setProgressBarValue(lVE.getProgressBarValue() + queryUsageStats.get(i).getTotalTimeInForeground());
                            check = true;
                        }
                    }

                    if (!check) {
                        listViewElements.add(new ListViewElements(queryUsageStats.get(i).getTotalTimeInForeground(), name));
                    }

                    bigSum += queryUsageStats.get(i).getTotalTimeInForeground();
                }
            } catch (IllegalArgumentException e) {
                e.printStackTrace();
            }
        }

        listViewElements.sort(Comparator.comparingDouble(ListViewElements:: getProgressBarValue));
        Collections.reverse(listViewElements);


        for (ListViewElements il: listViewElements) {
            Log.d("check", il.getTextViewString());
            Log.d("valueBefore", String.valueOf(il.getProgressBarValue()));
        }


        return listViewElements;
    }

    public ArrayList<ListViewElements> getDetailedUsageStats(ArrayList<ListViewElements> parameterList) {
        for (ListViewElements il: parameterList) {
            il.setTimeView(String.format("%d h %d m",
                    TimeUnit.MILLISECONDS.toHours((long)il.getProgressBarValue()),
                    TimeUnit.MILLISECONDS.toMinutes((long)il.getProgressBarValue()) -
                            TimeUnit.HOURS.toMinutes(TimeUnit.MILLISECONDS.toHours((long)il.getProgressBarValue()))
            ));
            if (bigSum != 0)
                il.setProgressBarValue(il.getProgressBarValue() * 10000 / bigSum);
            Log.d("check", il.getTextViewString());
            Log.d("valueBefore", String.valueOf(il.getProgressBarValue()));
        }
        return parameterList;
    }

    public boolean checkForPermission() {
        AppOpsManager appOps = (AppOpsManager) context.getSystemService(Context.APP_OPS_SERVICE);
        int mode = appOps.checkOpNoThrow(OPSTR_GET_USAGE_STATS, Process.myUid(), context.getPackageName());
        return mode == MODE_ALLOWED;
    }

}
