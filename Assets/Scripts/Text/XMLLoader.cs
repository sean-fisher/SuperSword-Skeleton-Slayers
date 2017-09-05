using UnityEngine;
using System.Collections;
using System.Collections.Generic; //Needed for Lists
using System.Xml; //Needed for XML functionality
using System.Xml.Serialization; //Needed for XML Functionality
using System.IO;
using System.Xml.Linq; //Needed for XDocument
using UnityEngine.SceneManagement;
using System.Linq;

public class XMLLoader : MonoBehaviour
{

    public static IEnumerable<XElement> convos;
    public static IEnumerable<XElement> battleMessages;
    XmlDocument xmlDoc; //create Xdocument. Will be used later to read XML file 

    public static XDocument dialogXDoc;

    void Start()
    {
    }

    void Update()
    {
    }

    public static void LoadCurrSceneDialog(string sceneName)
    {
        dialogXDoc = XMLLoader.LoadXML(sceneName);
        convos = XMLLoader.LoadConvos(dialogXDoc);
    }

    public static XDocument LoadXML(string sceneName, bool isDialogue = true)
    {
        TextAsset textAsset = null;
        if (isDialogue)
        {
            textAsset = (TextAsset)Resources.Load("Dialogue/Scenes/dia_" + sceneName);
        } else
        {
            textAsset = (TextAsset)Resources.Load("Battle/" + sceneName);
        }
        XmlDocument xmldoc = new XmlDocument();
        Debug.Log("Loaded " + sceneName + " dialogue.");
        if (textAsset)
        {
            xmldoc.LoadXml(textAsset.text);
            using (var nodeReader = new XmlNodeReader(xmldoc))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        } else
        {
            return null;
        }
    }

    public void PlayBattleMessage(int attackID, string attackerName = null, string targetName = null)
    {

    }

    // These short methods are to be used in a chain to get specific dialogue information from an XML doc

    public static IEnumerable<XElement> LoadConvos(XDocument xDoc)
    {
        if (xDoc != null)
        {
            return xDoc.Descendants("Conversation");
        } else
        {
            return null;
        }
    }

    public static XElement LoadConvo(IEnumerable<XElement> convos, int convoIndex)
    {
        if (convos.Count() > convoIndex)
        {
            return convos.ElementAt(convoIndex);
        } else
        {
            Debug.Log("Invalid convoIndex");
            return null;
        }
    }

    public static IEnumerable<XElement> GetLines(XElement convo)
    {
        return convo.Elements();
    }

    public static XElement GetLine(IEnumerable<XElement> lines, int lineNumber)
    {
        return lines.ElementAt(lineNumber);
    }

    public static string GetSpeakerName(XElement line)
    {
        return line.Elements().ElementAt(0).Value;
    }

    public static string GetSpeakerDialog(XElement line)
    {
        return line.Elements().ElementAt(1).Value;
    }
}

// This class is used to assign our XML Data to objects in a list so we can call on them later. 
/*public class XMLData {

    public int pageNum;
    public string charText, dialogueText;

    // Create a constructor that will accept multiple arguments that can be assigned to our variables. 
    public XMLData (int page, string character, string dialogue) {
        pageNum = page;
        charText = character;
        dialogueText = dialogue;
    }
}*/