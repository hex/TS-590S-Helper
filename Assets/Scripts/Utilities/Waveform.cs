using UnityEngine;
using System.Collections;
using UnityEngine.UI;
 
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(RawImage))]
public class Waveform : MonoBehaviour
{
     
     
	public int width = 500; // texture width 
	public int height = 100; // texture height 
	public Color backgroundColor = Color.black; 
	public Color waveformColor = Color.green; 
	public int size = 512; // size of sound segment displayed in texture
     
	private Color[] blank; // blank image array 
	private Texture2D texture; 
	private float[] samples; // audio samples array
     
	IEnumerator Start ()
	{ 
         
		// create the samples array 
		samples = new float[size]; 
         
		// create the texture and assign to the guiTexture: 
		texture = new Texture2D (width, height);
         
		GetComponent<RawImage>().texture = texture; 
         
		// create a 'blank screen' image 
		blank = new Color[width * height]; 
         
		for (int i = 0; i < blank.Length; i++) { 
			blank [i] = backgroundColor; 
		} 
         
		// refresh the display each 100mS 
		while (true) {
			GetCurWave (); 
			yield return new WaitForSeconds (0.1f); 
		} 
	}
     
	void GetCurWave ()
	{ 
		// clear the texture 
		texture.SetPixels (blank, 0); 
         
		// get samples from channel 0 (left) 
		GetComponent<AudioSource>().GetOutputData (samples, 0); 
         
		// draw the waveform 
		for (int i = 0; i < size; i++) { 
			texture.SetPixel ((int)(width * i / size), (int)(height * (samples [i] + 1f) / 2f), waveformColor);
		} // upload to the graphics card 
         
		texture.Apply (); 
	} 
}