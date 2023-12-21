using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;

namespace DomUtil {
    public class UssDescriptor {
        static readonly Type ELEMENT_BASE = typeof(VisualElement);
        static bool IsVisualElement(Type T) {
            return T.IsSubclassOf(ELEMENT_BASE) || T == ELEMENT_BASE;
        }
        public string[] classNames;
        public string id;
        public string tag;
        public UssDescriptor(string selector) {
            if (string.IsNullOrEmpty(selector)) {
                classNames = new string[0];
                return;
            }
            Compile(selector.Trim());
        }
        public UssDescriptor(string id = null, IEnumerable<string> @class = null, string tag = null) {
            this.id = id;
            classNames = @class?.Distinct().ToArray() ?? new string[0];
            this.tag = tag;
        }
        static readonly Regex pattern = new(@"^((?<Tag>[\w-]+)|#(?<Id>[\w-]+)|\.(?<Class>[\w-]+))+$", RegexOptions.Compiled);
        void Compile(string selector) {
            // ch = [A-Za-z0-0_-]
            // class = \.<ch>+  id = #.<ch>+  tag = <ch>+
            // selector = <tag>?<class>...<id>?<class>...
            // class +> class | id!, id +> class, tag +> class | id
            var m = pattern.Match(selector);
            if (!m.Success) {
                throw new UssSyntaxError($"Selector `{selector}` is invalid");
            }
            Group T = m.Groups["Tag"], I = m.Groups["Id"],  C = m.Groups["Class"];
            if (T.Success && T.Captures.Count == 1) {
                tag = T.Value;
            }
            if (I.Success) {
                if (I.Captures.Count > 1)
                    throw new UssSyntaxError($"Multiple id inside descriptor `{selector}`");
                id = I.Value;
            }
            if (C.Success) {
                classNames = C.Captures.Select(cap => cap.Value).ToArray();
            }
        }
        public VisualElement Q(VisualElement context) {
            var query = context.Query(id, classNames.ToArray());
            if (tag == null) {
                var elemType = ElemTypeByString(tag);
                query = query.Where(el => el.MatchesType(elemType));
            }
            return query.Build().First();
        }
        public UQueryBuilder<VisualElement> Query(VisualElement context) {
            var query = context.Query(id, classNames.ToArray());
            if (tag == null) {
                var elemType = ElemTypeByString(tag);
                query = query.Where(el => el.MatchesType(elemType));
            }
            return query;
        }
        public static Type ElemTypeByString(string typeName) {
            var ElemType = Type.GetType(typeName);
            if (ElemType == null)
                throw new NullReferenceException($"{typeName} does not exist");
            if (!IsVisualElement(ElemType))
                throw new Exception($"{typeName} is not a valid UIElement");
            return ElemType;
        }
        public static void Test() {
            var _A_B = new UssDescriptor(".aa.bb");
            Debug.Assert(_A_B.classNames.Contains("aa") && _A_B.classNames.Contains("bb"));
            var TagC_D = new UssDescriptor("CC.DD");
            Debug.Assert(TagC_D.tag == "CC" && TagC_D.classNames.Contains("DD"));
            var SharpE_F = new UssDescriptor("#EE.FF");
            Debug.Assert(SharpE_F.id == "EE" && SharpE_F.classNames.Contains("FF"));
            var _FSharpE = new UssDescriptor(".FF#EE");
            Debug.Assert(_FSharpE.id == "EE" && _FSharpE.classNames.Contains("FF"));
        }
    }

    public class UssDescriptor<T> : UssDescriptor where T : VisualElement {
        UssDescriptor(string id = null, IEnumerable<string> @class = null) {
            this.id = id;
            classNames = @class?.Distinct().ToArray() ?? new string[0];
            tag = nameof(T);
        }
        public new VisualElement Q(VisualElement context) {
            return context.Q(id, classNames.ToArray());
        }
        public new UQueryBuilder<VisualElement> Query(VisualElement context) {
            return context.Query(id, classNames.ToArray());
        }
    }

    internal class UssSyntaxError : System.Exception {
        public UssSyntaxError(string message) : base(message) {}
    }
}
