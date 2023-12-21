using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace DomUtil {
    public struct FourValueStyleLength : IEquatable<FourValueStyleLength> {
        /// <summary>
        /// Top or Top-Left
        /// </summary>
        public Length T;
        /// <summary>
        /// Right or Top-Right
        /// </summary>
        public Length R;
        /// <summary>
        /// Bottom or Bottom-Right
        /// </summary>
        public Length B;
        /// <summary>
        /// Bottom or Bottom-Left
        /// </summary>
        public Length L;
        public bool Equals(FourValueStyleLength other) {
            return T == other.T && R == other.R && B == other.B && L == other.L;
        }
        public static implicit operator FourValueStyleLength(float value) {
            return new FourValueStyleLength() {
                T = value, R = value, B = value, L = value
            };
        }
        public static implicit operator FourValueStyleLength(Length value) {
            return new FourValueStyleLength() {
                T = value, R = value, B = value, L = value
            };
        }
        public static implicit operator FourValueStyleLength((Length y, Length x) val) {
            return new FourValueStyleLength() {
                T = val.y, R = val.x, B = val.y, L = val.x
            };
        }
        public static implicit operator FourValueStyleLength((Length T, Length R, Length B) val) {
            return new FourValueStyleLength() {
                T = val.T, R = val.R, B = val.B, L = val.R
            };
        }
        public static implicit operator FourValueStyleLength((Length T, Length R, Length B, Length L) val) {
            return new FourValueStyleLength() {
                T = val.T, R = val.R, B = val.B, L = val.L
            };
        }
    }

    public static class StyleExtension {
        public static void setBorderRadius(this IStyle style,  FourValueStyleLength len) {
            style.borderTopLeftRadius     = len.T;
            style.borderTopRightRadius    = len.R;
            style.borderBottomLeftRadius  = len.B;
            style.borderBottomRightRadius = len.L;
        }
    }

    public static class CustomStyleExtension {
        // TODO: Support mixed length unit
        [Flags]
        public enum LengthUnitFlags {
            Pixel = 1, Percent = 2, Auto = 4, None = 8
        }
        public static readonly Regex SingleLengthInPixel =
            new(@"^([+-]?((\d+(\.\d*)?)|(\.\d+)))px$", RegexOptions.Compiled);
        public static readonly Regex SingleLengthInPercent =
            new(@"^([+-]?((\d+(\.\d*)?)|(\.\d+)))%$", RegexOptions.Compiled);
        public static bool TryGetPixelLength(this ICustomStyle self, CustomStyleProperty<Length> property, out Length value) {
            CustomStyleProperty<string> innerProp = new CustomStyleProperty<string>(property.name);
            if (self.TryGetValue(innerProp, out var str)) {
                var match = SingleLengthInPixel.Match(str);
                if (match.Success) {
                    var numStr = match.Groups[1].Value;
                    if (float.TryParse(numStr, out var num)) {
                        value = num;
                        return true;
                    }
                }
            }
            value = 0;
            return false;
        }
        public static bool TryGetPercentLength(this ICustomStyle self, CustomStyleProperty<Length> property, out Length value) {
            CustomStyleProperty<string> innerProp = new CustomStyleProperty<string>(property.name);
            if (self.TryGetValue(innerProp, out var str)) {
                var match = SingleLengthInPercent.Match(str);
                if (match.Success) {
                    var numStr = match.Groups[1].Value;
                    if (float.TryParse(numStr, out var num)) {
                        value = Length.Percent(num);
                        return true;
                    }
                }
            }
            value = Length.None();
            return false;
        }
        /// <summary>
        /// Read a custom style value within enum!
        /// </summary>
        public static bool TryGetValue<TEnum>(this ICustomStyle self, CustomStyleProperty<TEnum> property, out TEnum value) where TEnum : Enum {
            CustomStyleProperty<string> innerProp = new CustomStyleProperty<string>(property.name);
            if (self.TryGetValue(innerProp, out var str)) {
                if (Enum.TryParse(typeof(TEnum), str, out var vEnum)) {
                    value = (TEnum)vEnum;
                    return true;
                }
                Debug.LogWarning(
                    $"Cannot understand the value of {property.name}: {str}, valid choices are: " + string.Join(", ", Enum.GetNames(typeof(TEnum)))
                );
            }
            value = (TEnum)Enum.ToObject(typeof(TEnum), 0);
            return false;
        }
    }
    public static class ElementExtensions {
        /// <summary>
        /// Add given elements and return parent
        /// </summary>
        public static VisualElement AddElements(this VisualElement parent, params VisualElement[] children) {
            foreach (var el in children) {
                parent.Add(el);
            }
            return parent;
        }
        /// <summary>
        /// Add given elements and return parent
        /// </summary>
        public static VisualElement AddElements(this VisualElement parent, IEnumerable<VisualElement> children) {
            var childrenCopied = children.ToArray();
            return parent.AddElements(childrenCopied);
        }
        /// <summary>
        /// Append given child, return parent
        /// </summary>
        public static VisualElement Append(this VisualElement parent, VisualElement child) {
            parent.Add(child);
            return parent;
        }
        /// <summary>
        /// Append to given parent, return child
        /// </summary>
        public static VisualElement AppendTo(this VisualElement child, VisualElement parent) {
            parent.Add(child);
            return child;
        }
        /// <summary>
        /// Prepend given child, return parent
        /// </summary>
        public static VisualElement Prepend(this VisualElement parent, VisualElement child) {
            parent.Insert(0, child);
            return parent;
        }
        /// <summary>
        /// Prepend to given parent, return child
        /// </summary>
        public static VisualElement PrependTo(this VisualElement child, VisualElement parent) {
            parent.Insert(0, child);
            return child;
        }
        /// <summary>
        /// Add classes to element
        /// </summary>
        public static VisualElement AddClass(this VisualElement parent, params string[] classes) {
            foreach (var cls in classes) {
                parent.AddToClassList(cls);
            }
            return parent;
        }
        public static VisualElement AddClass(this VisualElement parent, IEnumerable<string> classes) {
            foreach (var cls in classes) {
                parent.AddToClassList(cls);
            }
            return parent;
        }
        public static VisualElement RemoveClass(this VisualElement parent, params string[] classes) {
            foreach (var cls in classes) {
                parent.RemoveFromClassList(cls);
            }
            return parent;
        }
        public static VisualElement RemoveClass(this VisualElement parent, IEnumerable<string> classes) {
            foreach (var cls in classes) {
                parent.RemoveFromClassList(cls);
            }
            return parent;
        }
        public static bool HasClass(this VisualElement elem, string className) {
            return elem.ClassListContains(className);
        }
        public static bool HasClass(this VisualElement elem, params string[] classNames) {
            foreach (var className in classNames) {
                if (elem.ClassListContains(className))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Begins with the current element, travels up the DOM tree until it finds a match for the supplied selector.
        /// A method borrowed from jQuery.
        /// </summary>
        public static VisualElement Closest(this VisualElement elem, string name = null, params string[] classes) {
            var current = elem;
            do {
                if (
                    (string.IsNullOrEmpty(name) || name == current.name) &&
                    (classes.Length == 0 || current.HasClass(classes))
                ) return current;
                current = current.parent;
            } while(current != null);
            return null;
        }
        /// <summary>
        /// Begins with the current element, travels up the DOM tree until it finds a match for the supplied selector.
        /// A method borrowed from jQuery.
        /// </summary>
        public static T Closest<T>(this VisualElement elem, string name = null, params string[] classes)
        where T : VisualElement {
            var current = elem;
            do {
                if (
                    (string.IsNullOrEmpty(name) || name == current.name) &&
                    (classes.Length == 0 || current.HasClass(classes)) &&
                    current is T
                ) return current as T;
                current = current.parent;
            } while(current != null);
            return null;
        }
        public static bool MatchesType(this VisualElement element, Type elemType) {
            if (elemType is null)
                return false;
            var T = element.GetType();
            return T == elemType || T.IsSubclassOf(elemType);
        }
    }

    /// <summary>
    /// .On / .Off are alias for .RegisterCallback / .UnregisterCallback, but event argument in callback is optional
    /// .Once registers handler that fires only once, event argument in callback is optional too.
    /// </summary>
    public static class ListenerRegisterExtension {
        /// <summary>
        /// Register handler that fires only once
        /// </summary>
        public static void Once<TEventType>(this CallbackEventHandler target, EventCallback<TEventType> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new() {
            if (callback == null) return;
            EventCallback<TEventType> realCallback = null;
            realCallback = (ev) => {
                callback(ev);
                target.UnregisterCallback<TEventType>(realCallback, useTrickleDown);
            };
            target.RegisterCallback<TEventType>(realCallback, useTrickleDown);
        }
        /// <summary>
        /// Register handler that fires only once
        /// </summary>
        public static void Once<TEventType>(this CallbackEventHandler target, Action callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new() {
            if (callback == null) return;
            target.Once<TEventType>((ev) => callback(), useTrickleDown);
        }
        /// <summary>
        /// Register handler that fires only once
        /// </summary>
        public static void Once<TEventType, TUserArgsType>(this CallbackEventHandler target, EventCallback<TEventType, TUserArgsType> callback, TUserArgsType userArgs, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new() {
            if (callback == null) return;
            EventCallback<TEventType, TUserArgsType> realCallback = null;
            realCallback = (ev, userArgs) => {
                callback(ev, userArgs);
                target.UnregisterCallback<TEventType, TUserArgsType>(realCallback, useTrickleDown);
            };
            target.RegisterCallback<TEventType, TUserArgsType>(realCallback, userArgs);
        }
        /// <summary>
        /// Register handler that fires only once
        /// </summary>
        public static void Once<TEventType, TUserArgsType>(this CallbackEventHandler target, Action callback, TUserArgsType userArgs, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new() {
            if (callback == null) return;
            target.Once<TEventType, TUserArgsType>((ev, userArgs) => callback(), userArgs, useTrickleDown);
        }
        /// <summary>
        /// Alias for RegisterCallback, but event argument in callback is optional
        /// </summary>
        public static void On<TEventType>(this CallbackEventHandler target, EventCallback<TEventType> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new() {
            target.RegisterCallback<TEventType>(callback, useTrickleDown);
        }
        /// <summary>
        /// Alias for RegisterCallback, but event argument in callback is optional
        /// </summary>
        public static void On<TEventType, TUserArgsType>(this CallbackEventHandler target, EventCallback<TEventType, TUserArgsType> callback, TUserArgsType userArgs, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new() {
            target.RegisterCallback<TEventType, TUserArgsType>(callback, userArgs, useTrickleDown);
        }
        /// <summary>
        /// Alias for RegisterCallback, but event argument in callback is optional
        /// </summary>
        public static void On<TEventType>(this CallbackEventHandler target, Action callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new() {
            if (callback == null) return;
            target.RegisterCallback<TEventType>((ev) => callback(), useTrickleDown);
        }
        /// <summary>
        /// Alias for RegisterCallback, but event argument in callback is optional
        /// </summary>
        public static void On<TEventType, TUserArgsType>(this CallbackEventHandler target, Action callback, TUserArgsType userArgs, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new() {
            if (callback == null) return;
            target.RegisterCallback<TEventType, TUserArgsType>((ev, userArgs) => callback(), userArgs, useTrickleDown);
        }
        /// <summary>
        /// Alias for UnregisterCallback, but event argument in callback is optional
        /// </summary>
        public static void Off<TEventType>(this CallbackEventHandler target, EventCallback<TEventType> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new() {
            target.UnregisterCallback<TEventType>(callback, useTrickleDown);
        }
        /// <summary>
        /// Alias for UnregisterCallback, but event argument in callback is optional
        /// </summary>
        public static void Off<TEventType, TUserArgsType>(this CallbackEventHandler target, EventCallback<TEventType, TUserArgsType> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new() {
            target.UnregisterCallback<TEventType, TUserArgsType>(callback, useTrickleDown);
        }
        /// <summary>
        /// Alias for UnregisterCallback, but event argument in callback is optional
        /// </summary>
        public static void Off<TEventType>(this CallbackEventHandler target, Action callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new() {
            if (callback == null) return;
            target.UnregisterCallback<TEventType>((ev) => callback(), useTrickleDown);
        }
        /// <summary>
        /// Alias for UnregisterCallback, but event argument in callback is optional
        /// </summary>
        public static void Off<TEventType, TUserArgsType>(this CallbackEventHandler target, Action callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new() {
            if (callback == null) return;
            target.UnregisterCallback<TEventType, TUserArgsType>((ev, userArgs) => callback(), useTrickleDown);
        }
        public static void OnChange<T>(this INotifyValueChanged<T> control, EventCallback<ChangeEvent<T>> callback) {
            if (callback == null || control == null) return;
            control.RegisterValueChangedCallback(callback);
        }
        public static void OffChange<T>(this INotifyValueChanged<T> control, EventCallback<ChangeEvent<T>> callback) {
            control.UnregisterValueChangedCallback(callback);
        }
        public static void OnceChange<T>(this INotifyValueChanged<T> control, EventCallback<ChangeEvent<T>> callback) {
            if (callback == null || control == null) return;
            EventCallback<ChangeEvent<T>> realCallback = null;
            realCallback = (ev) => {
                callback(ev);
                control.UnregisterValueChangedCallback(realCallback);
            };
            control.RegisterValueChangedCallback(callback);
        }
    }
}

public static class RectExtension {
    public static Vector2 topLeft(this Rect rect) {
        return new Vector2(rect.x, rect.y);
    }
    public static Vector2 topRight(this Rect rect) {
        return new Vector2(rect.x + rect.width, rect.y);
    }
    public static Vector2 bottomRight(this Rect rect) {
        return new Vector2(rect.x + rect.width, rect.y + rect.height);
    }
    public static Vector2 bottomLeft(this Rect rect) {
        return new Vector2(rect.x, rect.y + rect.height);
    }
}
