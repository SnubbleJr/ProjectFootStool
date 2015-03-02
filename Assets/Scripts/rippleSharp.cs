using UnityEngine;
using System.Collections;

public class rippleSharp : MonoBehaviour {

private int[] buffer1;
private int[] buffer2;
private int[] vertexIndices;

private Mesh mesh ;

private Vector3[] vertices ;
//private Vector3[] normals ;

public float dampner = 0.999f;
public float maxWaveHeight = 2.0f;

public int splashForce = 1000;

//public int slowdown = 20;
//private int slowdownCount = 0;
private bool swapMe = true;

public int cols = 128;
public int rows = 128;

	// Use this for initialization
void Start () {
		MeshFilter mf = (MeshFilter)GetComponent(typeof(MeshFilter));
		mesh = mf.mesh;
	    vertices = mesh.vertices;
		buffer1 = new int[vertices.Length];
		buffer2 = new int[vertices.Length];

    Bounds bounds = mesh.bounds;
    
    float xStep = (bounds.max.x - bounds.min.x)/cols;
    float yStep = (bounds.max.y - bounds.min.y)/rows;

	vertexIndices = new int[vertices.Length];	
    int i = 0;
	for (i = 0; i < vertices.Length; i++)
	{
		vertexIndices[i] = -1;
		buffer1[i] = 0;
		buffer2[i] = 0;
	}
    
    // this will produce a list of indices that are sorted the way I need them to 
    // be for the algo to work right
	for (i = 0; i < vertices.Length; i++) {
		float column = ((vertices[i].x - bounds.min.x)/xStep);// + 0.5;
		float row = ((vertices[i].y - bounds.min.y)/yStep);// + 0.5;
		float position = (row * (cols + 1)) + column + 0.5f;
		if (vertexIndices[(int)position] >= 0) print ("smash");
		vertexIndices[(int)position] = i;	
	}
	splashAtPoint(cols/2,rows/2);
}


void splashAtPoint(int x, int y) {
    //fix for inverted y

    int position = (((rows - y) * (cols + 1)) + x);
	buffer1[position] = splashForce;
    buffer1[position - 1] = splashForce;
	buffer1[position + 1] = splashForce;
	buffer1[position + (cols + 1)] = splashForce;
	buffer1[position + (cols + 1) + 1] = splashForce;
	buffer1[position + (cols + 1) - 1] = splashForce;
	buffer1[position - (cols + 1)] = splashForce;
	buffer1[position - (cols + 1) + 1] = splashForce;
	buffer1[position - (cols + 1) - 1] = splashForce;
}

// Update is called once per frame
void Update () {
	
	checkInput();
	
	int[] currentBuffer;
	if (swapMe) {
	// process the ripples for this frame
	    processRipples(buffer1,buffer2);
	    currentBuffer = buffer2;
	} else {
	    processRipples(buffer2,buffer1);		
	    currentBuffer = buffer1;
	}
	swapMe = !swapMe;
	// apply the ripples to our buffer
    Vector3[] theseVertices = new Vector3[vertices.Length];
 	int vertIndex;
 	int i = 0;
    for (i = 0; i < currentBuffer.Length; i++)
    {
    	vertIndex = vertexIndices[i];
        theseVertices[vertIndex] = vertices[vertIndex];
        theseVertices[vertIndex].z =  (currentBuffer[i] * 1.0f/splashForce) * maxWaveHeight;
    }
    mesh.vertices = theseVertices;


    // swap buffers		
}

void checkInput() {	
 if (Input.GetMouseButton (0)) {
	RaycastHit hit;
	if (Physics.Raycast (Camera.main.ScreenPointToRay(Input.mousePosition), out hit)) {
        Vector2 location = uVToCoOrds(hit.textureCoord);
        
	    splashAtPoint((int)location.x,(int)location.y);
    }
 }
}

Vector2 uVToCoOrds(Vector2 coOrds)
{
    //alerations I made to check input, given some coords from 0-1 gives the row and coloum needed
    Bounds bounds = mesh.bounds;
    float xStep = (bounds.max.x - bounds.min.x) / cols;
    float yStep = (bounds.max.y - bounds.min.y) / rows;
    float xCoord = (bounds.max.x - bounds.min.x) - ((bounds.max.x - bounds.min.x) * coOrds.x);
    float yCoord = (bounds.max.y - bounds.min.y) - ((bounds.max.y - bounds.min.y) * coOrds.y);
    float column = (xCoord / xStep);// + 0.5;
    float row = (yCoord / yStep);// + 0.5;

    return new Vector2((int)column, (int)row);
}

public void screenSplash(Vector2 coOrds)
{
    Vector2 location = uVToCoOrds(coOrds);

    splashAtPoint((int)location.x, (int)location.y);
}

void processRipples(int[] source, int[] dest) {
	int x = 0;
	int y  = 0;
	int position = 0;
	for ( y = 1; y < rows - 1; y ++) {
		for ( x = 1; x < cols ; x ++) {
			position = (y * (cols + 1)) + x;
			dest [position] = (((source[position - 1] + 
								 source[position + 1] + 
								 source[position - (cols + 1)] + 
								 source[position + (cols + 1)]) >> 1) - dest[position]);  
		   dest[position] = (int)(dest[position] * dampner);
		}			
	}	
}

}

