﻿using System;
using Xamarin.Forms;

namespace Goui.Forms
{
    public class VisualElementPackager : IDisposable
    {
        VisualElement _element;

        bool _isDisposed;

        IElementController ElementController => _element;

        public VisualElementPackager (IVisualElementRenderer renderer) : this (renderer, null)
        {
        }

        VisualElementPackager (IVisualElementRenderer renderer, VisualElement element)
        {
            if (renderer == null)
                throw new ArgumentNullException (nameof (renderer));

            Renderer = renderer;
            renderer.ElementChanged += OnRendererElementChanged;
            SetElement (null, element ?? renderer.Element);
        }

        protected IVisualElementRenderer Renderer { get; set; }

        public void Dispose ()
        {
            Dispose (true);
        }

        public void Load ()
        {
            for (var i = 0; i < ElementController.LogicalChildren.Count; i++) {
                var child = ElementController.LogicalChildren[i] as VisualElement;
                if (child != null)
                    OnChildAdded (child);
            }
        }

        protected virtual void Dispose (bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing) {
                SetElement (_element, null);
                if (Renderer != null) {
                    Renderer.ElementChanged -= OnRendererElementChanged;
                    Renderer = null;
                }
            }

            _isDisposed = true;
        }

        protected virtual void OnChildAdded (VisualElement view)
        {
            if (_isDisposed)
                return;

            //if (CompressedLayout.GetIsHeadless (view)) {
            //    var packager = new VisualElementPackager (Renderer, view);
            //    view.IsPlatformEnabled = true;
            //    packager.Load ();
            //}
            //else {
                var viewRenderer = Platform.CreateRenderer (view);
                Platform.SetRenderer (view, viewRenderer);

                var uiview = Renderer.NativeView;
                uiview.AppendChild (viewRenderer.NativeView);

                EnsureChildrenOrder ();
            //}
        }

        protected virtual void OnChildRemoved (VisualElement view)
        {
            if (_element == null)
                return;
            
            var viewRenderer = Platform.GetRenderer (view);
            if (viewRenderer == null || viewRenderer.NativeView == null)
                return;

            var parentRenderer = Platform.GetRenderer (_element);
            if (parentRenderer == null || parentRenderer.NativeView == null)
                return;

            parentRenderer.NativeView.RemoveChild (viewRenderer.NativeView);
        }

        void EnsureChildrenOrder ()
        {
            if (ElementController.LogicalChildren.Count == 0)
                return;

            for (var z = 0; z < ElementController.LogicalChildren.Count; z++) {
                var child = ElementController.LogicalChildren[z] as VisualElement;
                if (child == null)
                    continue;
                var childRenderer = Platform.GetRenderer (child);

                if (childRenderer == null)
                    continue;

                var nativeControl = childRenderer.NativeView;
#if __MOBILE__
                Renderer.NativeView.BringSubviewToFront(nativeControl);
#endif
                nativeControl.Style.ZIndex = z * 1000;
            }
        }

        void OnChildAdded (object sender, ElementEventArgs e)
        {
            var view = e.Element as VisualElement;
            if (view != null)
                OnChildAdded (view);
        }

        void OnChildRemoved (object sender, ElementEventArgs e)
        {
            var view = e.Element as VisualElement;
            if (view != null)
                OnChildRemoved (view);
        }

        void OnRendererElementChanged (object sender, VisualElementChangedEventArgs args)
        {
            if (args.NewElement == _element)
                return;

            SetElement (_element, args.NewElement);
        }

        void SetElement (VisualElement oldElement, VisualElement newElement)
        {
            if (oldElement == newElement)
                return;

            if (oldElement != null) {
                oldElement.ChildAdded -= OnChildAdded;
                oldElement.ChildRemoved -= OnChildRemoved;
                oldElement.ChildrenReordered -= UpdateChildrenOrder;

                if (newElement != null) {
                    var pool = new RendererPool (Renderer, oldElement);
                    pool.UpdateNewElement (newElement);

                    EnsureChildrenOrder ();
                }
                else {
                    var elementController = ((IElementController)oldElement);

                    for (var i = 0; i < elementController.LogicalChildren.Count; i++) {
                        var child = elementController.LogicalChildren[i] as VisualElement;
                        if (child != null)
                            OnChildRemoved (child);
                    }
                }
            }

            _element = newElement;

            if (newElement != null) {
                newElement.ChildAdded += OnChildAdded;
                newElement.ChildRemoved += OnChildRemoved;
                newElement.ChildrenReordered += UpdateChildrenOrder;
            }
        }

        void UpdateChildrenOrder (object sender, EventArgs e)
        {
            EnsureChildrenOrder ();
        }
    }
}
