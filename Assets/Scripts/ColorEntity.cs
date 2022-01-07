
/*****************************************************************************************************************
 - ColorEntity.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Breaking Walls
 Engine Version:     Unity 2018.2.10f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     This is the base class for any object that needs to change colors (i.e. wall, ball, etc.)
*****************************************************************************************************************/

using UnityEngine;

public class ColorEntity : MonoBehaviour
{
    #region Constants

    /// <summary>
    /// The default color of the entity, readonly. Currently the color of the wall is a light gray.
    /// </summary>
    public Color DefaultColor { get { return new Color(0.75f, 0.75f, 0.75f); } }
    
    // the three (four, if you count clear) colors in game, not accessible anywhere
    readonly Color[] COLORS = { Color.red, Color.green, Color.blue };

    #endregion


    /// <summary>
    /// This will determine if the wall is to be hidden or not.
    /// </summary>
    public bool Hidden { get; protected set; }

    /// <summary>
    /// The current color of the entity.
    /// </summary>
    public Color CurrentColor { get; protected set; }


    protected Material material;


    /// <summary>
    /// Sets the material reference to change its color.
    /// </summary>
    protected virtual void Awake()
    {
        material = GetComponent<Renderer>().material;
    }


    #region Color Mechanics

    /// <summary>
    /// Randomly selects a color from the Colors array and assigns it to the material of the object if there
    /// is a block to assign the color to.
    /// </summary>
    /// <param name="block"></param>
    protected void ChangeRandomColor()
    {
        SetBlockColor(Random.Range(0, COLORS.Length));
    }

    /// <summary>
    /// Changes the color based on the given index and if there is a valid. This function primarily 
    /// changes the color to the next element in Colors, or the first element if the current index 
    /// is at the last element of Colors. This method will not change the block color to
    /// glass, but the ChangeRandomColor() method will.
    /// 
    /// (I hope that made any sense)
    /// </summary>
    /// <param name="block"></param>
    /// <param name="index"></param>
    protected int ChangeColor(int index)
    {
        index = (index == COLORS.Length - 1) ? 0 : index + 1;
        SetBlockColor(index);

        return index;
    }

    /// <summary>
    /// Sets the color of the block based on the color index provided. This can only assign a color
    /// that is in the array.
    /// </summary>
    /// <param name="block"></param>
    /// <param name="index"></param>
    protected void SetBlockColor(int index)
    {
        if (index < COLORS.Length && index >= 0)
        {
            CurrentColor = COLORS[index];
            material.color = CurrentColor;
        }
    }

    /// <summary>
    /// Sets the specified block to the color provided. This can be any color, not just the colors
    /// provided in the COLORS array. Note: Color.clear will be converted to glass.
    /// </summary>
    /// <param name="block"></param>
    /// <param name="color"></param>
    protected void SetBlockColor(Color color)
    {
        CurrentColor = color;
        material.color = CurrentColor;
    }

    #endregion
}
