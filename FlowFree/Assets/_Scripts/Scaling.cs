using UnityEngine;

/// <summary>
/// 
/// This class is for scaling objects in a unity scene. 
/// Some functions scale objects using pìxels as reference 
/// and others changing unity scale. 
/// 
/// </summary>
public class Scaling
{
    // Reference resolution and current resolution
    Vector2 _refResolution;
    Vector2 _currResolution;

    // Factor to change between Unity Units and pixels
    float _unityUds;

    //----------------------------------------------------------
    //-----------------------Getters----------------------------
    //----------------------------------------------------------

    /// <summary>
    /// 
    /// Get the value to convert from pixels to Unity units and 
    /// viceversa.
    /// 
    /// </summary>
    /// <returns> (float) Previously calculated factor. </returns>
    public float TransformationFactor()
    {
        return _unityUds;
    } // TransformationFactor

    /// <summary>
    /// 
    /// Get the value of the actual Game resolution (width x Height)
    /// to use it for other operations
    /// 
    /// </summary>
    /// <returns> (Vector2) The value of the screen resolution. </returns>
    public Vector2 CurrentResolution()
    {
        return _currResolution;
    } // CurrentResolution

    //----------------------------------------------------------
    //-----------------------Getters----------------------------
    //----------------------------------------------------------

    /// <summary>
    /// 
    /// Class constructor. Receives the reference resolution to 
    /// use for scaling the different objects and the current 
    /// resolution of the screen. Also receives the size of the 
    /// camera and calculates the facto to translate pixels 
    /// to Unity units.
    ///  
    /// </summary>
    /// <param name="res"> Current screen resolution </param>
    /// <param name="refRes"> Resolution to use as reference </param>
    /// <param name="camSize"> Size in unity units of the camera </param>
    public Scaling(Vector2 res, Vector2 refRes, int camSize)
    {
        // Assign resolutions to intern vars.
        _currResolution = res;
        _refResolution = refRes;

        // Calculate how many pix3els per unity unit
        _unityUds = res.y / (2 * camSize);
    } // Scaling

    //----------------------------------------------------------
    //----------------------Utilities---------------------------
    //----------------------------------------------------------

    /// <summary>
    /// 
    /// Resize a number from the reference resolution to the current 
    /// screen resolution.
    /// 
    /// </summary>
    /// <param name="x"> (float) X value to resize. </param>
    /// <returns> (float) X value resized. </returns>
    public float ResizeX(float x)
    {
        return (x * _currResolution.x) / _refResolution.x;
    } // ResizeX

    /// <summary>
    /// 
    /// Resize a number from the reference resolution to the current 
    /// screen resolution.
    /// 
    /// </summary>
    /// <param name="y"> (float) Y value to resize. </param>
    /// <returns> (float) Y value resized. </returns>
    public float ResizeY(float y)
    {
        return (y * _currResolution.y) / _refResolution.y;
    } // ResizeY



    //----------------------------------------------------------
    //----------------------Functions---------------------------
    //----------------------------------------------------------

    /// <summary>
    /// 
    /// Transform some position in screen coordinates to world 
    /// coordinates, using current resolution and transformation
    /// factor. 
    /// 
    /// First check each of the coordinates if it is greater or 
    /// lower than the middle point (in screen values) and then 
    /// transform them using that information.
    /// 
    /// </summary>
    /// <param name="p"> (Vector2) Position to translate. </param>
    /// <returns> (Vector2) Position transformed to world pos. </returns>
    public Vector2 ScreenToWorldPosition(Vector2 p)
    {
        // Save position and work on a temporal value
        Vector2 temp = p;

        // Check if X position is higher than middle point
        if (temp.x > (_currResolution.x / 2))
        {
            // X positive, added to the middle point, then transform to Unity uds
            temp.x = (0 + (temp.x - (_currResolution.x / 2))) / _unityUds;
        } // if
        else
        {
            // X negative, substract to middle point and transform
            temp.x = (0 - ((_currResolution.x / 2) - temp.x)) / _unityUds;
        } // else

        // Check if Y position is higher than middle point
        if (temp.y > (_currResolution.y / 2))
        {
            // Y positive, added to the middle point, then transform to Unity uds
            temp.y = (0 + (temp.y - (_currResolution.y / 2))) / _unityUds;
        } // if
        else
        {
            // Y negative, substract to middle point and transform
            temp.y = (0 - ((_currResolution.y / 2) - temp.y)) / _unityUds;
        } // else

        return temp;
    } // ScreenToWorldPosition

    /// <summary>
    /// 
    /// Function that scales a sprite or a rectangle to fit the screen
    /// keeping its aspect ratio.
    /// 
    /// </summary>
    /// <param name="sizeInUnits"> (Vector3) Current size in Unity units. </param>
    /// <param name="scale"> (Vector3) Current scale of the object. </param>
    /// <returns> (Vector3) New scale of the object. </returns>
    public Vector3 ScaleToFitScreen(Vector3 sizeInUnits, Vector3 scale)
    {
        // Temporal variable to save state
        Vector3 temp = sizeInUnits;

        // Convert units to pixels
        temp.x *= _unityUds;
        temp.y *= _unityUds;

        // Set object's width to screen's width
        temp.x = _currResolution.x;

        // Calculate height proportionally
        temp.y = (temp.x * sizeInUnits.y) / sizeInUnits.x;

        // Convert back to Unity Units
        temp.x /= _unityUds;
        temp.y /= _unityUds;

        // Calculate new scale using function
        Vector3 nScale = ResizeObjectScale(sizeInUnits, temp, scale);

        return nScale;
    } // ScaleToFitScreen

    /// <summary>
    /// 
    /// Scales a rectangle using another as reference keeping the 
    /// aspect ratio of the first one.
    /// 
    /// </summary>
    /// <param name="srcDims"> (Vector2) Rectangle tobe scaled. </param>
    /// <param name="refDims"> (Vector2) Reference rectangle to scale. </param>
    /// <returns> (Vector2) New scaled dimensions. </returns>
    public Vector2 ScaleKeepingAspectRatio(Vector2 srcDims, Vector2 refDims)
    {
        Vector2 temp = srcDims;

        // CHeck width and scale keeping aspect ratio
        if (temp.x > refDims.x || temp.x < refDims.x)
        {
            // Set width to fit new rectangle
            temp.x = refDims.x;

            // Scale height proportionally
            temp.y = (temp.x * srcDims.y) / srcDims.x;
        } // if

        // Check height
        if (temp.y > refDims.y)
        {

            // If scalated already, reboot
            if (temp != srcDims)
            {
                temp = srcDims;
            } // if

            temp.y = refDims.y;

            // Scale width proportionally
            temp.x = (temp.y * srcDims.x) / srcDims.y;
        } // if

        return temp;
    } // ScaleKeepingAspectRation


    /// <summary>
    /// 
    /// Calculate a new scale of an object keeping aspect ratio.
    /// 
    /// </summary>
    /// <param name="orUnits"> (Vector3) Units that object occupies currently. </param>
    /// <param name="currUnits"> (Vector3) The target units to scale the object. </param>
    /// <param name="scale"> (Vector3) Current scale of the object to use as reference. </param>
    /// <returns> (Vector3) New calculated scale. </returns>
    public Vector3 ResizeObjectScale(Vector3 orUnits, Vector3 currUnits, Vector3 scale)
    {
        // Create new vecotr to calculate new scale
        Vector3 scalated = new Vector3();

        // Check width of the object
        // Calculate new scale
        scalated.x = scalated.y = (currUnits.x * scale.x) / orUnits.x;

        // check height
        if (orUnits.y > currUnits.y)
        {
            // If new scale is calculated, reboot it
            if (scalated.y != 0 && scalated.y != 0)
            {
                scalated.x = scalated.y = 0;
            } // if

            // Calculate new scale
            scalated.y = scalated.x = (currUnits.y * scale.y) / orUnits.y;
        }

        return scalated;
    } // ResizeObjectScale

} // Scaling
