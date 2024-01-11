#if (UNITY_EDITOR)
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class EditorStartUp
{
    //After how many editor startups should we ask the user for a review
    private static int AskReviewAfterStartupAmount = 3;
    //Only show the popup after the editor has recompiled within the first 30 seconds of launching.
    private static int AskReviewTimeThreshold = 30;

    static EditorStartUp()
    {
        EditorApplication.delayCall += WaitForInspectorUpdate;
    }

    private static void WaitForInspectorUpdate()
    {
        AskForReview();
    }

    private static void AskForReview()
    {
        //If this is called within the first 'AskReviewTimeThreshold' seconds of the Unity Editor being opened.
        if (EditorApplication.timeSinceStartup <= AskReviewTimeThreshold)
        {
            //If the 'ShouldAskForReview_TerraTiler2D' key does not exist, or if the value is true
            if (!EditorPrefs.HasKey("ShouldAskForReview_TerraTiler2D") || EditorPrefs.GetBool("ShouldAskForReview_TerraTiler2D"))
            {
                //If the 'ToolOpenedAmount_TerraTiler2D' key does not exist
                if (!EditorPrefs.HasKey("ToolOpenedAmount_TerraTiler2D"))
                {
                    //Add the key and set the value to 1
                    EditorPrefs.SetInt("ToolOpenedAmount_TerraTiler2D", 1);
                }
                //If the 'ToolOpenedAmount_TerraTiler2D' key does exist
                else
                {
                    //Add 1 to the value of the key
                    EditorPrefs.SetInt("ToolOpenedAmount_TerraTiler2D", EditorPrefs.GetInt("ToolOpenedAmount_TerraTiler2D") + 1);
                }

                //If this project has been opened 'AskReviewAfterStartupAmount' times or more
                if (EditorPrefs.GetInt("ToolOpenedAmount_TerraTiler2D") >= AskReviewAfterStartupAmount)
                {
                    //Show a popup that asks the user for a review
                    int dialogResponse = UnityEditor.EditorUtility.DisplayDialogComplex(
                        "TerraTiler2D Review",
                        "Have you been enjoying TerraTiler2D so far? \n\nIt would be greatly appreciated if you could leave me a review! \n\nThanks for your support!\n - Robo Bonobo 😃",
                        "Leave a review",
                        "Remind me later",
                        "Don't ask again");

                    //If the user clicked "Leave a review"
                    if (dialogResponse == 0)
                    {
                        Application.OpenURL("https://assetstore.unity.com/packages/tools/terrain/terratiler2d-198030#reviews");
                        //Never show this popup again.
                        EditorPrefs.SetBool("ShouldAskForReview_TerraTiler2D", false);
                    }
                    //If the user clicked "Remind me later", or closed the popup otherwise
                    else if (dialogResponse == 1)
                    {
                        //Set the amount of times this project has been opened to 0
                        EditorPrefs.SetInt("ToolOpenedAmount_TerraTiler2D", 0);
                        EditorPrefs.SetBool("ShouldAskForReview_TerraTiler2D", true);
                    }
                    //If the user clicked "Don't ask again"
                    else if (dialogResponse == 2)
                    {
                        //Save their response, and never show this popup again.
                        EditorPrefs.SetBool("ShouldAskForReview_TerraTiler2D", false);
                    }
                }
            }
        }
    }
}
#endif