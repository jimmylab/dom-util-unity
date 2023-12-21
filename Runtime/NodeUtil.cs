using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DomUtil {
    // Defination in summary (typescript-like syntax)
    // H<T extends VisualElement = VisualElement> : T (
    //     @ref   ?: out T,    // Optional
    //     @class ?: string | IEnumerable<string>,
    //     id     ?: string,
    //     init   ?: Action<T>,
    //     child  ?: VisualElement | IEnumerable<VisualElement>
    // )
    public static class NodeUtil {
        /// <summary>
        /// Util function to generate a bunch of element,
        /// inspired by h() function that widely adopted in many frontend libs.
        /// </summary>
        /// <param name="class">class name(s) of element</param>
        /// <param name="id">Name of element ("id" equivalent in html)</param>
        /// <param name="init">Init callback, custom assign operations goes here</param>
        /// <param name="child">Child elements</param>
        /// <typeparam name="T">Element type to create, default = VisualElement</typeparam>
        /// <returns></returns>
        public static T H<T>(
            out T r,
            IEnumerable<string> @class,
            string id = "",
            Action<T> init = null,
            IEnumerable<VisualElement> child = null
        ) where T : VisualElement, new() {
            var elem = new T();
            if (@class != null)
                foreach (var clsName in @class) elem.AddToClassList(clsName);
            if (id != null)
                elem.name = id;
            if (child != null)
                foreach (var kid in child) elem.Add(kid);
            init?.Invoke(elem);
            r = elem;
            return elem;
        }

        /// <inheritdoc cref="H{T}(out T, IEnumerable{string}, string, Action{T}, IEnumerable{VisualElement})"/>
        public static T H<T>(
            IEnumerable<string> @class,
            string id = "",
            Action<T> init = null,
            IEnumerable<VisualElement> child = null
        ) where T : VisualElement, new() {
            return H<T>(out _, @class, id, init, child);
        }

        public static T H<T>(
            string @class = null,
            string id = null,
            Action<T> init = null,
            IEnumerable<VisualElement> child = null
        ) where T : VisualElement, new() {
            var classList = (@class != null) ? new [] {@class} : new string[]{};
            return H<T>(classList, id, init, child);
        }

        public static VisualElement H(
            string @class = null,
            string id = null,
            Action<VisualElement> init = null,
            IEnumerable<VisualElement> child = null
        ) {
            return H<VisualElement>(@class, id, init, child);
        }
        public static VisualElement H(
            IEnumerable<string> @class = null,
            string id = null,
            Action<VisualElement> init = null,
            IEnumerable<VisualElement> child = null
        ) {
            return H<VisualElement>(@class, id, init, child);
        }

        public static T H<T>(
            T elem,
            IEnumerable<string> @class,
            string id = "",
            Action<T> init = null,
            IEnumerable<VisualElement> child = null
        ) where T : VisualElement, new() {
            if (@class != null)
                foreach (var clsName in @class) elem.AddToClassList(clsName);
            if (id != null)
                elem.name = id;
            if (child != null)
                foreach (var kid in child) elem.Add(kid);
            init?.Invoke(elem);
            return elem;
        }

        public static T H<T>(
            T elem,
            string @class = null,
            string id = "",
            Action<T> init = null,
            IEnumerable<VisualElement> child = null
        ) where T : VisualElement, new() {
            var classList = (@class != null) ? new [] {@class} : new string[]{};
            return H<T>(elem, classList, id, init, child);
        }

        static void example() {
            H("AA", child: new TextElement[] {
                // You cannot omit new **TextElement**
                // since the Button doesn't inherit from Label, and vice versa,
                // otherwise c# cannot determine the best type of array (Fuck C#).
                // 你不能省略new后面的TextElement，因为Button与Label不是继承关系，所以必须显式声明，否则C#找不到数组最佳类型(傻逼C#)。
                H<Button>(init: el => el.text = "button"),
                H<Label>(new []{"BB", "CC"}, init: el => { el.text = "text"; }),
            });
        }
    }

    public static class VisualElementExtensions {
        public static void Add(this VisualElement parent, params VisualElement[] children) {
            foreach (var child in children) {
                parent.Add(child);
            }
        }

        public static bool IsVisualElement(Type ElemType) {
            return ElemType == typeof(VisualElement) ||
                   ElemType.IsSubclassOf(typeof(VisualElement));
        }
        public static UQueryBuilder<VisualElement> _Query(
            this VisualElement el,
            Type ElemType,
            string name = null,
            params string[] classes
        ) {
            if (!IsVisualElement(ElemType))
                throw new Exception($"{ElemType} is not VisualElement");
            if (el == null)
                throw new ArgumentNullException(nameof(el));

            return (UQueryBuilder<VisualElement>)(
                el.GetType()
                .GetMethod("Query")?.MakeGenericMethod(ElemType)
                .Invoke(el, new object[]{ name, classes })
            );
            //return ReflectionUtils
            //    .GetMethod(el, "Query")
            //    .Generic(ElemType)
            //    .ResultAs<UQueryBuilder<VisualElement>>();
        }
    }
}
