﻿//
//  Emoji.Wpf — Emoji support for WPF
//
//  Copyright © 2017—2018 Sam Hocevar <sam@hocevar.net>
//
//  This library is free software. It comes without any warranty, to
//  the extent permitted by applicable law. You can redistribute it
//  and/or modify it under the terms of the Do What the Fuck You Want
//  to Public License, Version 2, as published by the WTFPL Task Force.
//  See http://www.wtfpl.net/ for more details.
//

using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

using Controls = System.Windows.Controls;

namespace Emoji.Wpf
{
    /// <summary>
    /// A simple WPF text control that is emoji-aware.
    /// </summary>
    public partial class TextBlock : Controls.TextBlock
    {
        static TextBlock()
        {
            TextProperty.OverrideMetadata(typeof(TextBlock), new FrameworkPropertyMetadata(
                (string)Controls.TextBlock.TextProperty.GetMetadata(typeof(Controls.TextBlock)).DefaultValue,
                (o, e) => (o as TextBlock).OnTextChanged(e.NewValue as string)));

            ForegroundProperty.OverrideMetadata(typeof(TextBlock), new FrameworkPropertyMetadata(
                (Brush)ForegroundProperty.GetMetadata(typeof(Controls.TextBlock)).DefaultValue,
                (o, e) => (o as TextBlock).OnForegroundChanged(e.NewValue as Brush)));

            FontSizeProperty.OverrideMetadata(typeof(TextBlock), new FrameworkPropertyMetadata(
                (double)FontSizeProperty.GetMetadata(typeof(Controls.TextBlock)).DefaultValue,
                (o, e) => (o as TextBlock).OnFontSizeChanged((double)e.NewValue)));
        }

        public TextBlock()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Override System.Windows.Controls.TextBlock.Text using our own dependency
        /// property. This is necessary because we do not want the parent callbacks
        /// to run, ever, and OverrideMetadata does not properly hide them.
        /// Also note that calling GetValue/SetValue is a lot faster when used directly
        /// on the DependencyPropertyDescriptor because it bypasses the reflection code.
        /// </summary>
        public new string Text
        {
            get => m_text_dpd.GetValue(this) as string;
            set => m_text_dpd.SetValue(this, value);
        }

        /// <summary>
        /// Override System.Windows.Controls.TextBlock.TextProperty
        /// </summary>
        public static new readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextBlock));

        private void OnTextChanged(string text)
        {
            Inlines.Clear();
            if (string.IsNullOrEmpty(text))
                return;

            int pos = 0;
            foreach (Match m in EmojiData.MatchMultiple.Matches(text))
            {
                Inlines.Add(text.Substring(pos, m.Index - pos));
                Inlines.Add(new EmojiInline()
                {
                    FallbackBrush = Foreground,
                    Text = text.Substring(m.Index, m.Length),
                    FontSize = FontSize,
                });

                pos = m.Index + m.Length;
            }
            Inlines.Add(text.Substring(pos));
        }

        private void OnForegroundChanged(Brush brush)
        {
            foreach (var inline in Inlines)
                if (inline is EmojiInline)
                    (inline as EmojiInline).Foreground = brush;
        }

        private void OnFontSizeChanged(double size)
        {
            foreach (var inline in Inlines)
                if (inline is EmojiInline)
                    (inline as EmojiInline).FontSize = size;
        }

        private static readonly DependencyPropertyDescriptor m_text_dpd =
            DependencyPropertyDescriptor.FromProperty(TextProperty, typeof(TextBlock));
    }
}

