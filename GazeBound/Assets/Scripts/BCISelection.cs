using Gtec.Chain.Common.Templates.Utilities;
using Gtec.UnityInterface;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BCISelection : MonoBehaviour
{

    [Header("Feedback")]
    [SerializeField] private Image flashOverlay; // full-screen UI Image, set color in Inspector

    public void OnClassificationSelection(ERPPipeline erpPipeline, ClassSelection classSelection)
    {
        if (classSelection == null) return;

        Debug.Log(">>> BCI TRIGGERED — Class: " + classSelection.Class);
        StartCoroutine(FlashScreen());
    }

    private IEnumerator FlashScreen()
    {
        flashOverlay.gameObject.SetActive(true);
        flashOverlay.color = Color.green; // bright green flash

        yield return new WaitForSeconds(0.5f);

        flashOverlay.gameObject.SetActive(false);
    }

}