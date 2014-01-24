angular.module('coevery.wizard', [])
    .directive('coWizard', function($compile, $dialog, $state) {
        return {
            template: '<div class="hide" ui-view></div>',
            replace: false,
            restrict: 'A',
            scope: { options: '=wizardOptions' },
            link: function(scope, element, attrs) {
                var template = $('<div class="modal-header"><button type="button" class="close">&times;</button><h3></h3></div><div class="modal-body"></div><div class="modal-footer"><button class="btn">Back</button><button class="btn">Next</button><button class="btn btn-primary">Save</button></div>'),
                    templateContent = template.filter('.modal-body'),
                    closeBtn = templateContent.prev().children(':first'),
                    title = closeBtn.next(),
                    backBtn = templateContent.next().children(':first'),
                    nextBtn = backBtn.next(),
                    saveBtn = nextBtn.next(),
                    index = -1,
                    dialogOptions = {
                        backdrop: true,
                        backdropFade: true,
                        dialogFade: true,
                        backdropClick: false,
                        keyboard: true,
                        template: getTemplate,
                    },
                    titleText = scope.options.title,
                    closeFunc = scope.options.closeFunc,
                    completeFunc = scope.options.completeFunc,
                    states = scope.options.states;

                title.text(titleText);

                scope.$on('$viewContentLoaded', function() {
                    index = $.inArray($state.current.name, states);
                    if (index > -1) {
                        handleButtons();
                        if (scope.dialog) {
                            addTemplateContent();
                        } else {
                            scope.dialog = $dialog.dialog(dialogOptions);
                            scope.dialog.open();
                        }
                    }
                });

                scope.$watch('dialog._open', function(newValue, oldValue) {
                    if (newValue == false && oldValue == true) {
                        closeWizard();
                    }
                });

                function getTemplate() {
                    bindTemplateEvents();
                    addTemplateContent();
                    return template;
                }

                function bindTemplateEvents() {
                    closeBtn.click(closeWizard);
                    backBtn.click(handlePrevious);
                    nextBtn.click(handleNext);
                    saveBtn.click(handleSave);
                }

                function addTemplateContent() {
                    templateContent.empty();
                    templateContent.append(element.find('[ui-view]').contents());
                }

                function closeWizard() {
                    closeFunc();
                    scope.dialog.close();
                    scope.dialog = null;
                }

                function handleButtons() {
                    index == 0 ? backBtn.hide() : backBtn.show();
                    index == states.length - 1
                        ? nextBtn.hide() && saveBtn.show()
                        : nextBtn.show() && saveBtn.hide();
                }

                function handlePrevious() {
                    if (index > 0) {
                        var context = {};
                        scope.$broadcast('wizardGoBack', context);
                        if (context.cancel) {
                            return;
                        }
                        $state.transitionTo(states[index - 1], context.stateParams);
                    }
                }

                function handleNext() {
                    if (index < states.length - 1) {
                        var context = {};
                        scope.$broadcast('wizardGoNext', context);
                        if (context.cancel) {
                            return;
                        }
                        $state.transitionTo(states[index + 1], context.stateParams);
                    }
                }

                function handleSave() {
                    if (index == states.length - 1) {
                        var context = {};
                        scope.$broadcast('wizardComplete', context);
                        if (context.cancel) {
                            return;
                        }
                        closeWizard();
                        completeFunc && completeFunc();
                    }
                }
            }
        };
    });