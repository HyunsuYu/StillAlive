using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NPCLookPart", menuName = "StillAlive/NPCLookPart")]
public class NPCLookPart : ScriptableObject
{
    [Serializable] public struct ColorPalette
    {
        [Serializable] public struct LookPartColors
        {
            public Color RedRegionColor;
            public Color BlueRegionColor;
            public Color GreenRegionColor;
            public Color RedGreenRegionColor;
            public Color RedBlueRegionColor;
            public Color GreenBlueRegionColor;
            public Color RedGreenBlueRegionColor;
        }


        public LookPartColors TopColors;
        public LookPartColors FacesColors;
        public LookPartColors EyesColors;
        public LookPartColors MouthsColors;
        public LookPartColors GlassesColors;
        public LookPartColors CapsColors;
        public LookPartColors FrontHairsColors;
        public LookPartColors BackHairsColors;
    }


    [Header("Fixed Characters")]
    public Sprite MainCharacter;

    [Header("Random Characters")]
    public Sprite[] Tops;
    public Sprite[] Faces;
    public Sprite[] Eyes;
    public Sprite[] Mouths;
    public Sprite[] Glasses;
    public Sprite[] Caps;
    public Sprite[] FrontHairs;
    public Sprite[] BackHairs;


    public ColorPalette[] ColorPalettes;
}