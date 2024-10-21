using FancyToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridBoard
{
    public class TileData
    {
        public TileVisuals visuals;
        public string idVisuals;
        public string id;
        public string title;
        public string description;
        public bool isEmpty;
        public int startingLevel;
        public int cost;
        public List<string> tags;
        public Tile.Type type;


        // generic variables, use by extending classes
        public int priority;
        public Rarity rarity;

        public string onClick;
        public List<string> onDragAccept;
        public string defaultState;
        public bool canDrag;

        public string colorCode;

        public virtual string GetDescription(Tile tile = null)
        {
            return description;
        }
        
        public bool HasTag(string tag)
            => tag == "any"
            || type.ToString().ToLower() == tag
            || tags.Contains(tag);
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
