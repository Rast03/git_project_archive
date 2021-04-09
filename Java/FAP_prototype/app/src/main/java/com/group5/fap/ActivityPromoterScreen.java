package com.group5.fap;

import android.content.Context;
import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.TextView;
import com.mindorks.placeholderview.SwipeDecor;
import com.mindorks.placeholderview.SwipePlaceHolderView;


public class ActivityPromoterScreen extends AppCompatActivity {

    private int[] activityImages = new int[] {
            R.drawable.amager_beach,
            R.drawable.copenhagen_zoo,
            R.drawable.experimentarium,
            R.drawable.fishing,
            R.drawable.kings_garden,
            R.drawable.rosenborg_castle,
            R.drawable.rundetarn,
            R.drawable.the_blue_planet,
            R.drawable.the_incredibles_2,
            R.drawable.tivoli
    };

    private String[] activityNames = new String[] {
            "Amager beach",
            "Copenhagen zoo",
            "Experimentarium",
            "Fishing",
            "Kings garden",
            "Rosenborg castle",
            "Rundetårn",
            "The Blue Planet",
            "The Incredibles 2",
            "Tivoli"
    };

    private String[] activityDescription = new String[] {
            "Amager strand er et perfekt sted at tage hen hvis man lyster sig stranden. Hvis vejret et godt og solen skinner er dette det bedste sted ag nyde det.",
            "Afrika, Amerika eller Asien? Tag til Københavns Zoo hvis du har lyst til at udforske dyreverden. Her er der mange slags dyr som dig og familien kan se. Du kan endda komme helt tæt på en Isbjørn og snart en Panda!",
            "Besøg eksperimentariet I København! Her findes der mange slags fysik/kemi forsøg. Bliv smartere på en dag!",
            "Lyst til at fiske? Tag til Nordhavnen og fisk sammen med mange andre lystfiskere. Det eneste du har brug for er nogle regnorme og en fiskestang.",
            "Slap af I Kongens Have, som er bygget af Christian d. 4. Her findes der mange skulpturer og blomster. Dette sted er det perfekte lokation for en picnic.",
            "Lyst til at udforske et slot? Rosenberg slot er det perfekte sted at tag hen hvis man har lyst til at udforske hvordan en konge boede. Du kan gå ind i selve slottet og ud i haven.",
            "Besøg et af Danmarks mest ikoniske bygninger. Rundetårn er et perfekt sted at tag op i for at få en god udsigt over København.",
            "Har du undret dig over hvilke slags væsner som lever under havet? Den Blå Planet som ligger i København, har du mulighed for at udforske fiskene som lever under vandet. Akvariet er nord Europas største med en samling af 20.000 dyr. Hvis du har lyst til at dykke under vandet og se hvilke væsner havfruen Ariel lever sammen med så tag til Den Blå Planet.",
            "Den længe ventede film er ankommet! Tag ind og se De Utrolige 2 i biografen!",
            "Tag en tur I Tivoli som er et af Danmarks mest populære forlystelsespark. Du kan tag en tur i dæmonen eller går en tur i haven. Tivoli er verdens anden ældste forlystelsespark. Du kan også tag til fredagsrock hvor kunstnere spiller musik."
    };




    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_promoter_screen);

        SwipePlaceHolderView swipeView = (SwipePlaceHolderView)findViewById(R.id.swipeView);
        Context context = getApplicationContext();

        swipeView.getBuilder()
                .setDisplayViewCount(1)
                .setSwipeDecor(new SwipeDecor()
//                        .setPaddingTop(5)
//                        .setRelativeScale(0.01f)
                        .setSwipeInMsgLayoutId(R.layout.tinder_swipe_in_msg_view)
                        .setSwipeOutMsgLayoutId(R.layout.tinder_swipe_out_msg_view));

        TextView textView = (TextView) findViewById(R.id.scrollableText);

        boolean goToResults = false;
        for (int i = 0; i < activityNames.length; i++) {
            if (i == activityNames.length-1)
                goToResults = true;
            swipeView.addView(new TinderCard(context, activityImages[i], activityNames[i], activityDescription[i], textView, swipeView, goToResults));
        }


        findViewById(R.id.rejectBtn).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Log.d("CHECK", "NO");
                swipeView.doSwipe(false);
            }
        });

        findViewById(R.id.acceptBtn).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Log.d("CHECK", "YES");
                swipeView.doSwipe(true);
            }
        });
    }

    public void onClick(View view) {
        startActivity(new Intent(view.getContext(), HomeScreen.class));
    }

}
