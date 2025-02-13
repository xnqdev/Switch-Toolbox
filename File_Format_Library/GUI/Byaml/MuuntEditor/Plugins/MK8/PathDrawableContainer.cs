﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using FirstPlugin.Turbo.CourseMuuntStructs;

namespace FirstPlugin.MuuntEditor
{
    public class PathDrawableContainer : PropertyObject, I2DDrawableContainer
    {
        public Color PathColor;

        public PathDrawableContainer(string name, Color color) {
            Name = name;
            PathColor = color;
        }

        public List<BasePathGroup> PathGroups
        {
            get
            {
                var groups = new List<BasePathGroup>();
                foreach (var group in SubObjects)
                    groups.Add((BasePathGroup)group);
                return groups;
            }
        }

        private IDrawableObject drawable;
        public IDrawableObject Drawable
        {
            get
            {
                if (drawable == null)
                    drawable = new RenderablePath(PathGroups, PathColor); 

                return drawable;
            }
        }
    }
}
