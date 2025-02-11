using FancyToolkit;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;

namespace GridBoard
{
    public class TileData : DataWithId, IHasInfo, IInfoTextProvider, IIconProvider, IHintProvider, IHintContainer, IHoverInfoTarget
    {
        public const string anyTag = "any";
        public const string nonEmpty = "nonempty";

        public Sprite sprite;
        public string idVisuals;
        public string title;
        public string description;
        public bool isEmpty;
        public int startingLevel;
        public int cost;
        public List<string> tags = new();
        public Tile.Type type;


        // generic variables, use by extending classes
        public int priority;
        public Rarity rarity;

        public string onClick;
        public List<string> onDragAccept;
        public string defaultState;
        public bool canDrag;

        public string colorCode;

        public virtual string GetDescription(Tile tile)
        {
            return description;
        }

        public virtual string GetDescription() => GetDescription(null);

        public virtual IHasInfo GetExtraInfo() => null;

        public virtual List<IHintProvider> GetHintProviders() => new();
        public virtual string GetHintText() => description;

        public Sprite GetIcon() => sprite;

        public virtual string GetInfoText(int size) => description;

        public List<string> GetTags()
        {
            var result = tags.ToList();
            if (type != Tile.Type.None)
            {
                result.Add(type.ToString().ToLower());
            }

            return result;
        }

        public string GetTitle() => title;
        public virtual List<string> GetTooltips() => new();

        public bool HasTag(string tag)
            => tag == anyTag
            || tag == id
            || (tag == nonEmpty && id != "empty")
            || type.ToString().ToLower() == tag
            || tags.Contains(tag);

        public virtual bool ShouldShowHoverInfo() => !isEmpty;

        public bool ShouldShowInfo() => !isEmpty;
    }

    public abstract class TileAction
    {
        public virtual string GetDescription() => "";
        public abstract bool Run(Tile tile);
    }

    public abstract class TileActionAccept
    {
        public abstract bool CanAccept(Tile tile, Tile target);
        public abstract void Accept(Tile tile, Tile target);
    }

    public abstract class TileState
    {
        protected Tile parent;
        public virtual void Enter(Tile tile) { parent = tile; }
        public virtual void Exit() { }
        public abstract void Update();

        public virtual string GetDescription() => "";
    }
}
