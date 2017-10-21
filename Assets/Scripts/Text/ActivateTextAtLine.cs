using UnityEngine;
using System.Collections;

public class ActivateTextAtLine : MonoBehaviour {

    public TextAsset activeText;

    public int startLine;
    public int endLine;

    public TextBoxManager textManager;

    public bool destroyWhenFinished;
    public bool isBarrier;
    private bool repel;
    //private int repelCount = 0;

    public bool requireButtonPress;
    private bool waitForPress;

	// Use this for initialization
	void Start () {
        textManager = FindObjectOfType<TextBoxManager>();
	}
	
	// Update is called once per frame
	void Update () {
        if (waitForPress && Input.GetKeyDown(KeyCode.X))
        {
            waitForPress = false;
            textManager.ReloadScript(this.activeText);
            textManager.currentLine = startLine;
            textManager.endLine = endLine;
            //textManager.EnableTextBox();

            

            if (destroyWhenFinished)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.name == "Cop Player Good") && !textManager.GetAlreadyPressed())
        {
            Debug.Log("Bad enable");
            if (requireButtonPress)
            {
                waitForPress = true;
                return;
            }

            textManager.ReloadScript(this.activeText);
            textManager.currentLine = startLine;
            textManager.endLine = endLine;
            //textManager.EnableTextBox();
            
            if (destroyWhenFinished)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.name == "Cop Player")
        {
            waitForPress = false;
        }
    }
}
