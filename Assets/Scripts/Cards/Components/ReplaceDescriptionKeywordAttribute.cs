using System;

/// <summary>
/// Put this attribute on a non-static method that has no parameters and returns string to mark it as a method that will replace a <b>description keyword</b>. <para/>
/// 
/// <b>Example:</b> If we have a description that says <i>"Deals {damage} points of damage"</i> then the <i>"{damage}"</i> part of the description is considered a <b>description keyword</b>.<br/>
/// If you now add one of these attributes on a method in a <see cref="CardComponent"/> script, then the returned <see cref="string"/> of the method is what will replace the <i>"{damage}"</i> <b>description keyword</b>. <para/>
/// 
/// <code>
/// [SerializeField] private float dmg;
/// 
/// [ReplaceDescriptionKeyword("damage")]
/// private string ReplaceDescriptionKeyword()
/// {
///     // Replace the "damage" keyword with how much damage this CardComponent does
///     return dmg.ToString();
/// }
/// </code>
/// 
/// Adding a ReplaceDescriptionKeyword attribute without any parameters like this: <c>[ReplaceDescriptionKeyword]</c> will instead replace the attributes keyword parameter with the <see cref="CardComponent"/>s name in the inspector.
/// </summary>
// Script by Ruben
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class ReplaceDescriptionKeywordAttribute : Attribute
{
    /// <summary>
    /// The description keyword that this attribute is going to replace. <para/>
    /// If this value is null, then this attribute is instead replacing a description keyword with the same name as the <see cref="CardComponent"/> script it's on.
    /// </summary>
    public string Keyword => _keyword;
    private string _keyword;

    public ReplaceDescriptionKeywordAttribute()
    {
        _keyword = null;
    }

    public ReplaceDescriptionKeywordAttribute(string keyword)
    {
        _keyword = keyword;
    }
}
