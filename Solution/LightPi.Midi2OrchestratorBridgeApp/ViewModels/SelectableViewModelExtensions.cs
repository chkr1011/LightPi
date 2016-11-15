using System;
using System.Collections.Generic;

namespace LightPi.Midi2OrchestratorBridge.ViewModels
{
    public static class SelectableViewModelExtensions
    {
        public static void SelectMatching<TModel>(
            this IEnumerable<SelectableViewModel<TModel>> source,
            Predicate<SelectableViewModel<TModel>> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            foreach (var item in source)
            {
                item.IsSelected = predicate(item);
            }
        }
    }
}
