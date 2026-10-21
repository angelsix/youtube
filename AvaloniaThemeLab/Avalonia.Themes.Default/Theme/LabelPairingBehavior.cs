using System;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.LogicalTree;

namespace Avalonia.Themes.Default;

/// <summary>
/// Marks the <see cref="Label"/> that names a text field stacked directly under it, so the theme
/// can pull the two together instead of leaving them looking like two separate blocks.
/// </summary>
/// <remarks>
/// <para><b>Why a behaviour:</b> a Label says "this text names that field" in two ways, and NEITHER
/// of them is reachable from a selector. <see cref="Label.Target"/> is the semantic one — the
/// developer points the Label at the field it names — but a selector cannot ask "is Target set?":
/// a property match compares a CONVERTED VALUE, and Target holds an <c>IInputElement</c>, which no
/// string converts to, so such a selector refuses to compile. The positional one — the Label sits
/// directly above the field — has no selector at all: Avalonia has no adjacent-sibling combinator,
/// and <c>:nth-child</c> counts from the start of the parent, so it cannot say "the one after me"
/// either. So the theme reads both signals here, where code can, and reports the answer as a
/// pseudo-class the Label's own ControlTheme styles.</para>
/// <para><b>Which signal wins:</b> <see cref="Label.Target"/> first, because it is the explicit
/// statement of intent; the control directly after the Label is the fallback, for the common
/// caption-over-a-field stack where nobody bothered to wire a Target. A Label that names
/// something that is not a text field (a button, a group box, a slider) is not paired and keeps
/// its padding.</para>
/// <para><b>Which direction pulls:</b> only a field stacked DIRECTLY UNDER the Label, in the same
/// panel, in the same column (a Grid) or as the very next child (a vertical StackPanel). A field
/// BESIDE the Label is a different arrangement and keeps its padding: a horizontal pairing is
/// aligned by the panel, and dropping the label's bottom padding there would push its text off
/// the field's baseline.</para>
/// <para>The attached property carries a BARE name ("Apply"): Avalonia 12 rejects a dotted
/// registration name ('name' may not contain periods), and the owner type supplies uniqueness.
/// The Label ControlTheme sets it on every Label, so the pairing is a property of the control
/// rather than something a page has to opt into.</para>
/// </remarks>
public sealed class LabelPairing
{
    /// <summary>Attaches the pairing detection to a <see cref="Label"/>.</summary>
    public static readonly AttachedProperty<bool> ApplyProperty =
        AvaloniaProperty.RegisterAttached<LabelPairing, Label, bool>("Apply");

    // The pseudo-class the Label ControlTheme reads: "^:paired". A pseudo-class rather than a
    // style class, because a style class a ControlTheme defines has to be declared in a Styles
    // collection for the IDE to resolve it (ThemeRules, Rule 17), and this theme carries none.
    private const string PairedPseudoClass = ":paired";

    // One live attachment per Label: the weak key keeps a detached Label from pinning its
    // subscriptions (mirrors WheelSnap.Live).
#pragma warning disable IDE0028 // The repo's analyzers reject both spellings of this initializer.
    private static readonly ConditionalWeakTable<Label, Attachment> live = new();
#pragma warning restore IDE0028

    private sealed class Attachment
    {
    }

    public static void SetApply(Label target, bool value) => target.SetValue(ApplyProperty, value);

    public static bool GetApply(Label target) => target.GetValue(ApplyProperty);

    /// <summary>
    /// The type initializer subscribes to the flag's GLOBAL CHANGE OBSERVABLE
    /// (<c>AvaloniaProperty.Changed</c>: "fired when this property changes on ANY instance"), so
    /// the behaviour activates from either direction: the compiled XAML applies the flag with
    /// <c>SetValue</c> — which fires the observable — and <see cref="SetApply"/>, which a consumer
    /// may call, touches this type and so arms the initializer.
    /// </summary>
    static LabelPairing()
    {
        try
        {
            ApplyProperty.Changed.Subscribe(new FlagWatcher());
        }
        catch
        {
            // Headless oddities must never take the app down.
        }
    }

    private sealed class FlagWatcher : IObserver<AvaloniaPropertyChangedEventArgs>
    {
        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(AvaloniaPropertyChangedEventArgs value)
        {
            if (value.Sender is not Label target)
                return;
            try
            {
                if (GetApply(target))
                    Attach(target);
            }
            catch
            {
                // Wiring trouble must never propagate into the shared application.
            }
        }
    }

    private static void Attach(Label label)
    {
        if (live.TryGetValue(label, out _))
            return;
        live.GetValue(label, static _ => new Attachment());

        // The theme's flag arrives as the Label joins the tree, so the parent and the Target are
        // usually known already. Re-check on Loaded, because a Target written by NAME resolves
        // against the name scope, which can land after the first evaluation, and re-check whenever
        // the Target changes.
        label.Loaded += (_, _) => Evaluate(label);
        label.PropertyChanged += (_, args) =>
        {
            if (args.Property == Label.TargetProperty)
                Evaluate(label);
        };

        Evaluate(label);
    }

    /// <summary>Decides whether the Label names a text field under it, and reports it as a pseudo-class.</summary>
    private static void Evaluate(Label label)
    {
        try
        {
            var named = LabelledControl(label);
            bool paired = named is not null && IsTextField(named) && IsDirectlyBelow(label, named);
            ((IPseudoClasses)label.Classes).Set(PairedPseudoClass, paired);
        }
        catch
        {
            // Any surprise leaves the Label as it was rather than taking the app down.
        }
    }

    /// <summary>
    /// The control the Label names: its <see cref="Label.Target"/> when it has one, else the control
    /// directly after it.
    /// </summary>
    /// <remarks>
    /// A Target is the developer's explicit statement, so it is the ONLY answer when it is set: a
    /// Label pointing at a button names a button, and the field that happens to sit under it is
    /// not the thing it labels.
    /// </remarks>
    private static Control? LabelledControl(Label label)
    {
        if (label.Target is Control target)
            return target;
        return NextSibling(label);
    }

    /// <summary>The control that follows the Label among its parent's logical children.</summary>
    private static Control? NextSibling(Label label)
    {
        if (label.Parent is not ILogical parent)
            return null;

        bool seen = false;
        foreach (ILogical child in parent.LogicalChildren)
        {
            if (seen)
                return child as Control;
            if (ReferenceEquals(child, label))
                seen = true;
        }
        return null;
    }

    /// <summary>
    /// Whether the named control is stacked directly under the Label, in the panel they share.
    /// </summary>
    /// <remarks>
    /// The panel decides, not the geometry, so the answer needs no layout pass and cannot flicker
    /// as the layout settles. A vertical <see cref="StackPanel"/> stacks its children in order, so
    /// the field must be the very next child. A <see cref="Grid"/> places children by row and
    /// column, so the field must be one row below in the same column. Any other panel lays out
    /// freely, and the theme does not guess.
    /// </remarks>
    private static bool IsDirectlyBelow(Label label, Control candidate)
    {
        if (label.Parent is null || !ReferenceEquals(candidate.Parent, label.Parent))
            return false;

        return label.Parent switch
        {
            StackPanel { Orientation: Orientation.Vertical } => ReferenceEquals(NextSibling(label), candidate),
            Grid => Grid.GetRow(candidate) == Grid.GetRow(label) + 1
                    && Grid.GetColumn(candidate) == Grid.GetColumn(label),
            _ => false,
        };
    }

    /// <summary>
    /// The text fields a Label can name: the plain text box (and the masked box, which derives
    /// from it) and the fields built on one (the numeric spinner, the auto-completing box). A
    /// drop-down, a picker, a slider and a button are not text fields, so a Label naming one keeps
    /// its padding.
    /// </summary>
    private static bool IsTextField(Control control) =>
        control is TextBox or NumericUpDown or AutoCompleteBox;
}
