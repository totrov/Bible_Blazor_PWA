﻿using Bible_Blazer_PWA.Components.Interactor.Home;
using Bible_Blazer_PWA.Components.Interactor.LessonUnits;
using MudBlazor;
using System;
using System.Collections.Generic;
using static Bible_Blazer_PWA.Components.Interactor.LessonsInUnit.Interaction.LessonsInUnitInteractionModel;
using static Bible_Blazer_PWA.Components.Interactor.LessonsInUnit.Interaction;
using Bible_Blazer_PWA.Services.Menu;
using Bible_Blazer_PWA.Pages.Lesson;

namespace Bible_Blazer_PWA.Components.Interactor.Lesson
{
    public class LessonInteractionModel : InteractionModelBase<LessonInteractionModel>
    {
        public override bool IsSide => true;

        public override bool ShouldPersistInHistory => true;

        public override Type ComponentType => typeof(LessonInteractionComponent);

        public event Action OnRefreshNeeded;
        protected void RefreshNeeded() => OnRefreshNeeded?.Invoke();

        #region Overrides
        public override bool NeedRerenderOnModelChangeImpl(LessonInteractionModel prevModel)
        {
            return prevModel.LessonNumber != LessonNumber || prevModel.UnitId != UnitId;
        }
        #endregion

        #region Buttons
        public override IEnumerable<(IButtonStateHandler, IButtonVisibilityHandler)> GetButtons()
        {
            IconResolver iconReolver = new IconResolver();

            yield return (new LessonLevelHandler(iconReolver, RefreshNeeded), null);
            yield return (new BibleRefToggleHandler(iconReolver, RefreshNeeded), new CustomVisibilityHandler(() => Interaction.GetParameters().HideBibleRefTabs != "True"));
            yield return (new FontSizeIncreaseHandler(iconReolver, RefreshNeeded), null);
            yield return (new FontSizeDecreaseHandler(iconReolver, RefreshNeeded), null);
            yield return (new NotesEditModeToggleHandler(iconReolver, RefreshNeeded), null);
        }
        #endregion

        #region Breadcrumb processing

        BreadcrumbsFacade.BreadcrumbRecord lastBreadcrumb;

        public override IEnumerable<BreadcrumbsFacade.BreadcrumbRecord> GetBreadcrumbs()
        {
            yield return new BreadcrumbsFacade.BreadcrumbRecord
            {
                Text = "",
                Action = () =>
                {
                    HomeInteractionModel.ApplyToCurrentPanel(this);
                },
                Icon = Icons.Material.Filled.Home
            };

            yield return new BreadcrumbsFacade.BreadcrumbRecord
            {
                Text = "Планы уроков",
                Action = () =>
                {
                    LessonUnitsInteractionModel.ApplyToCurrentPanel(this);
                },
                Icon = null
            };

            yield return new BreadcrumbsFacade.BreadcrumbRecord
            {
                Text = UnitId,
                Action = () =>
                {
                    LessonsInUnitInteractionModel.WithParameters<SelectedUnitId>
                        .ApplyToCurrentPanel(new(UnitId), this);
                },
                Icon = null
            };

            lastBreadcrumb = new BreadcrumbsFacade.BreadcrumbRecord
            {
                Text = "",
                Action = () =>
                {
                    LessonInteractionModel.WithParameters<UnitIdLessonId>
                        .ApplyToCurrentPanel(new(UnitId, LessonNumber), this);
                },
                Icon = null
            };

            yield return lastBreadcrumb;
        }

        public void SetLessonName(string name) => lastBreadcrumb.Text = name;

        #endregion

        #region Parameters

        public string UnitId { get; set; }
        public string LessonNumber { get; set; }
        public int[] ElementId { get; set; }

        public class UnitIdLessonId : Parameters
        {
            public string UnitId { get; set; }
            public string LessonNumber { get; set; }
            public UnitIdLessonId(string unitId, string lessonNumber)
            {
                UnitId = unitId;
                LessonNumber = lessonNumber;
            }

            public override void ApplyParametersToModel(LessonInteractionModel model)
            {
                model.UnitId = UnitId;
                model.LessonNumber = LessonNumber;
            }
        }

        public class UnitIdLessonIdElementId : UnitIdLessonId
        {
            public int[] ElementId { get; set; }
            public UnitIdLessonIdElementId(string unitId, string lessonNumber, int[] elementId)
                :base(unitId, lessonNumber)
            {
                ElementId = elementId;
            }
            public override void ApplyParametersToModel(LessonInteractionModel model)
            {
                base.ApplyParametersToModel(model);
                model.ElementId = ElementId;
            }
        }
        #endregion
    }
}
