package com.group5.fap;


import android.content.Context;
import android.content.Intent;
import android.util.Log;
import android.widget.ImageView;
import android.widget.ProgressBar;
import android.widget.TextView;
import com.bumptech.glide.Glide;
import com.mindorks.placeholderview.SwipePlaceHolderView;
import com.mindorks.placeholderview.annotations.Layout;
import com.mindorks.placeholderview.annotations.Resolve;
import com.mindorks.placeholderview.annotations.View;
import com.mindorks.placeholderview.annotations.swipe.SwipeCancelState;
import com.mindorks.placeholderview.annotations.swipe.SwipeIn;
import com.mindorks.placeholderview.annotations.swipe.SwipeInState;
import com.mindorks.placeholderview.annotations.swipe.SwipeOut;
import com.mindorks.placeholderview.annotations.swipe.SwipeOutState;

import java.util.concurrent.ThreadLocalRandom;

import static android.support.v4.content.ContextCompat.startActivity;

@Layout(R.layout.tinder_card_view)
public class TinderCard {

    @View(R.id.profileImageView)
    private ImageView profileImageView;

    @View(R.id.nameAgeTxt)
    private TextView nameAgeTxt;

    @View(R.id.progressBar)
    private ProgressBar progressBar;

    private int activityImages;
    private String activityNames;
    private String activityDescription;
    private TextView textView;
    private Context context;
    private SwipePlaceHolderView swipeView;
    private boolean goToResults;

    public TinderCard(Context context, int activityImages, String activityNames, String activityDescription, TextView textView, SwipePlaceHolderView swipeView, boolean goToResults) {
        this.context = context;
        this.activityImages = activityImages;
        this.activityNames = activityNames;
        this.activityDescription = activityDescription;
        this.textView = textView;
        this.swipeView = swipeView;
        this.goToResults = goToResults;
    }

    @Resolve
    private void onResolved(){
        Glide.with(context).load(activityImages).into(profileImageView);
        nameAgeTxt.setText(activityNames);
        progressBar.setProgress(ThreadLocalRandom.current().nextInt(100));
        textView.setText(activityDescription);


    }

    @SwipeOut
    public void onSwipedOut(){
        Log.d("EVENT", "onSwipedOut");
        if (goToResults)
            startActivity(context, new Intent(context, ActivityResultsScreen.class), null);

    }

    @SwipeCancelState
    private void onSwipeCancelState(){
        Log.d("EVENT", "onSwipeCancelState");
    }

    @SwipeIn
    public void onSwipeIn(){
        Log.d("EVENT", "onSwipedIn");
        if (goToResults)
            startActivity(context, new Intent(context, ActivityResultsScreen.class), null);

    }

    @SwipeInState
    private void onSwipeInState(){
        Log.d("EVENT", "onSwipeInState");
    }

    @SwipeOutState
    private void onSwipeOutState(){
        Log.d("EVENT", "onSwipeOutState");
    }

}


