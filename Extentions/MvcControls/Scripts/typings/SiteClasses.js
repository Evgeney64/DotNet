var VmBase = /** @class */ (function () {
    function VmBase() {
    }
    Object.defineProperty(VmBase.prototype, "is_AutoTest_Logging", {
        get: function () {
            if (Top().$('#checkbox_auto_test_logging')[0].checked)
                return true;
            else
                return false;
        },
        enumerable: false,
        configurable: true
    });
    Object.defineProperty(VmBase.prototype, "mainTabstrip", {
        get: function () {
            if ($("#tabstrip").length > 0)
                return $("#tabstrip");
            else
                return Top().$("#tabstrip");
        },
        enumerable: false,
        configurable: true
    });
    VmBase.prototype.initializeArguments = function (funcArguments, rewrite) {
        var func_str = funcArguments.callee.toString(); //Получаем исходный код функции в виде строки
        var args = func_str.split("(")[1].split(")")[0].trim().split(","); //Берем оттуда только список пришедших аргументов
        for (var i = 0; i < args.length; i++) {
            if (vm_Base[args[i].trim()] != undefined && vm_Base[args[i].trim()] != "" && (funcArguments[i] === "" || funcArguments[i] == undefined))
                continue;
            if (args[i].trim() != "" && args[i].trim() != undefined) //Все остальные аргументы функции преобразуем в свойства объекта vm_Base
                vm_Base[args[i].trim()] = funcArguments[i];
        }
        if (rewrite != undefined) { //Парсим строку vm_Base_params со свойствами vm_Base с сервера и преобразуем в свойства нового объекта vm_Base
            var params = vm_Base.vm_Base_params.split("|");
            for (var j = 0; j < params.length - 1; j++) {
                if (vm_Base[params[j].split(":")[0]] == undefined || vm_Base[params[j].split(":")[0]] != params[j].split(":")[1])
                    vm_Base[params[j].split(":")[0]] = params[j].split(":")[1];
            }
        }
        if (vm_Base.url_Action != undefined && Top().isAutoTestClient == true) {
            vm_Base.url_Action = addToUrl(vm_Base.url_Action, "isAutoTest", true);
            //if (vm_Base.is_AutoTest_Logging == true) {
            //Не получается передавать сразу в updateQueryStringParameter функцию check_AutoTest_Logging(). Для true/false не работает
            vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "isAutoTestLogging", vm_Base.is_AutoTest_Logging);
            //}
            //else {
            //    vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "isAutoTestLogging", vm_Base.is_AutoTest_Logging);
            //}
        }
        if (vm_Base.url_Content == undefined && Top().vm_Base.url_Content != undefined)
            vm_Base.url_Content = Top().vm_Base.url_Content;
        if (clickItem.sender == undefined || clickItem.fromVmBase || clickItem.fromMainMenu) {
            clickItem.clear();
            for (var prop in this) {
                clickItem[String(prop)] = this[prop];
            }
            clickItem.fromVmBase = true;
        }
    };
    VmBase.prototype.getFromParams = function (param_name) {
        var params = vm_Base.vm_Base_params.split("|");
        for (var j = 0; j < params.length - 1; j++) {
            if (params[j].split(":")[0] == param_name)
                return params[j].split(":")[1];
        }
    };
    return VmBase;
}());
var vm_Base = new VmBase();
//Глобальный класс, который содержит в себе все input'ы, измененные input'ы, проверенные input'ы и прочее
var Editor = /** @class */ (function () {
    function Editor() {
    }
    return Editor;
}());
var editor = new Editor();
var FormDataNew = /** @class */ (function () {
    function FormDataNew() {
        //Убрать. Сделать как в clickItem'е через prop
        this.keys = new Array();
    }
    Object.defineProperty(FormDataNew.prototype, "formDataObject", {
        get: function () {
            var t = new FormData();
            for (var i = 0; i < this.keys.length; i++)
                t.append(this.keys[i], this[this.keys[i]]);
            return t;
        },
        enumerable: false,
        configurable: true
    });
    FormDataNew.prototype.toFormDataObject = function () {
        var t = new FormData();
        for (var prop in this)
            t.append(prop, this[String(prop)]);
        return t;
    };
    //Добавление нового свойства в formData
    FormDataNew.prototype.append = function (name, value) {
        if (this.keys.indexOf(name) == -1) {
            this.keys[this.keys.length] = name;
        }
        this[name] = value;
    };
    ;
    //Считывание всех input'ов
    FormDataNew.prototype.readInputs = function (formName, group) {
        if (document.forms[formName] == undefined)
            return;
        editor.inputs = new Array();
        editor.inputsChanged = new Array();
        editor.inputsIsRequired = new Array();
        editor.editableGridItems = new Array();
        this.clearFormData();
        //Возвращение всех элементов формы по заданному селектору
        var all_inputs = document.forms[formName].querySelectorAll("input.ctrl-input[id], textarea.ctrl-input[id], select.ctrl-input[id], li.ctrl-input input, span.ctrl-input[id]");
        if (group != undefined && group != -1)
            all_inputs = document.forms[formName].querySelectorAll("input.ctrl-input[group='" + group + "'], textarea.ctrl-input[group='" + group + "']");
        for (var i = 0; i < all_inputs.length; i++) {
            editor.inputs[editor.inputs.length] = all_inputs[i];
            if (all_inputs[i].getAttribute("Changed") != null)
                editor.inputsChanged[editor.inputsChanged.length] = all_inputs[i];
            if (all_inputs[i].getAttribute("isrequired") != null)
                editor.inputsIsRequired[editor.inputsIsRequired.length] = all_inputs[i];
        }
        var editable_grids = document.forms[formName].getElementsByClassName("grid k-editable");
        for (var i = 0; i < editable_grids.length; i++) {
            if (editable_grids[i].getAttribute("grid_is_editable").length > 0) {
                entity_ReadData_EditableGrid(editable_grids[i], editor.editableGridItems);
            }
        }
        var editor_grids = document.forms[formName].getElementsByClassName("grid k-reorderable");
        for (var i = 0; i < editor_grids.length; i++) {
            if (editor_grids[i].getAttribute("name") != null
                && editor_grids[i].getAttribute("name").length > 0
                && getRowId_ByGuid(editor_grids[i].getAttribute("page_guid")) != null) {
                var grid_row = { name: editor_grids[i].getAttribute("name"), value: getRowId_ByGuid(editor_grids[i].getAttribute("page_guid")) };
                editor.inputsChanged[editor.inputsChanged.length] = grid_row;
            }
        }
        this.checkRequred();
        this.inputsToFormData();
    };
    ;
    //Проверка всех input'ов
    FormDataNew.prototype.checkRequred = function () {
        if (editor.inputsIsRequired.length > 0) { //Проверка на заполнение обязательных input'ов
            for (var i = 0; i < editor.inputsIsRequired.length; i++) {
                var control = editor.inputsIsRequired[i];
                if (control.getAttribute("controltype") != "Upload" && control.getAttribute("controltype") != "Selector") {
                    if (control.value == null || control.value == "" || control.getAttribute("Changed") == undefined) {
                        if (this.errorMessage == undefined)
                            this.errorMessage = "<div>Не задано поле: &nbsp[" + control.title + "]</div>";
                        else
                            this.errorMessage += "<div>Не задано поле: &nbsp[" + control.title + "]</div>";
                    }
                }
                if (control.getAttribute("controltype") == "Upload") {
                    if ($(control).data("kendoUpload").getFiles().length == 0 || control.getAttribute("Changed") == undefined)
                        if (this.errorMessage == undefined)
                            this.errorMessage = "<div>Не загружены файлы для поля: " + control.getAttribute("title") + "</div>";
                        else
                            this.errorMessage += "<div>Не загружены файлы для поля: " + control.getAttribute("title") + "</div>";
                }
                if (control.getAttribute("controltype") == "Selector") {
                    var selector_id = $("#val_" + control.getAttribute("page_guid")).attr("selector_id");
                    if (selector_id == undefined || control.getAttribute("Changed") == undefined) {
                        if (this.errorMessage == undefined)
                            this.errorMessage = "<div>Не задано поле: &nbsp[" + control.title + "]</div>";
                        else
                            this.errorMessage += "<div>Не задано поле: &nbsp[" + control.title + "]</div>";
                    }
                }
            }
        }
        if (editor.inputsChanged.length > 0) { //Остальные проверки
            for (var i = 0; i < editor.inputsChanged.length; i++) {
                //Проверки файлов
                var control = editor.inputsChanged[i];
                if (control.attributes == undefined) { //Если это не элемент разметки, а строка грида
                    continue;
                }
                if (control.getAttribute("controltype") == "Upload") {
                    var files = $(control).data("kendoUpload").getFiles();
                    if (files.length != 0) {
                        //проверяем количество
                        if (files.length > 20) {
                            this.errorMessage += "<div>Не более 20 файлов!</div>";
                        }
                        // проверяем размер
                        for (var j = 0; j < files.length; j++) {
                            if (files[j].rawFile.size > 71680000) {
                                this.errorMessage += "<div>Файлы не более 70 мегабайт!</div>";
                            }
                        }
                    }
                }
                if (control.getAttribute("not_check_value") == undefined || control.getAttribute("not_check_value") == "false") {
                    if (control.getAttribute("controltype") != "HtmlEditor" && control.value != undefined && (control.value.indexOf(">") != -1 || control.value.indexOf("<") != -1)) {
                        this.errorMessage += "<div>Введен недопустимый символ (\"<\", \">\")</div><div></div>";
                    }
                }
            }
        }
    };
    ;
    //Заполнение глобальной formData. Может дополняться обычными переменными через метод append()
    FormDataNew.prototype.inputsToFormData = function () {
        if (editor.inputsChanged.length > 0) {
            for (var i = 0; i < editor.inputsChanged.length; i++) {
                var control = editor.inputsChanged[i];
                if (control.attributes == undefined) { //Если это не элемент разметки, а строка грида
                    this.append(control.name, control.value);
                    continue;
                }
                if (control.type == "file") { //fileUpload
                    var files = $(control).data("kendoUpload").getFiles();
                    for (var j = 0; j < files.length; j++) {
                        this.append(control.name + ".(" + j + ")", files[j].rawFile);
                    }
                    this.append($(control).data("kendoUpload").name, control.getAttribute("name"));
                    continue;
                }
                if (control.type == "checkbox") { //Чекбоксы
                    if (control.getAttribute("iscurrentdatecheckbox") == "1") { //Чекбокс "Действующие"
                        //let check_control = $("#check_" + control.getAttribute("page_guid"))[0];
                        //if (check_control == undefined)
                        //    continue;
                        //if (check_control.getAttribute("checked") == "checked")
                        //    this.append(control.name, "on");
                        //else
                        //    this.append(control.name, "off");
                        this.append(control.name, "on");
                        continue;
                    }
                    else {
                        if (control.checked) //Обычные
                            this.append(control.name, "on");
                        else
                            this.append(control.name, "off");
                        continue;
                    }
                }
                if (control.type == "select-multiple") { //МультиКомбоБокс
                    this.append(control.name, $(control).data("kendoMultiSelect").value());
                    continue;
                }
                if (control.getAttribute("istext") != null //текст
                    && control.getAttribute("istext").length > 0
                    && control.getAttribute("istext") == "True") {
                    this.append(control.getAttribute("name"), control.innerText);
                    continue;
                }
                if (control.getAttribute("isdaterange") != null //Диапазон дат
                    && control.getAttribute("isdaterange").length > 0
                    && control.getAttribute("isdaterange") == "True") {
                    if (control.getAttribute("idvalend") != null) {
                        this.append(control.name, control.value + ";" + this.getInputById(control.getAttribute("idvalend")).value); //Значения двух дат объединяем в одно
                    }
                    continue;
                }
                if (control.getAttribute("idvalend") != null //Числовой диапазон
                    && control.getAttribute("idcombobox") != null) {
                    formData.append(control.name, control.value + ";" + this.getInputById(control.getAttribute("idvalend")).value + ";" + this.getInputById(control.getAttribute("idcombobox")).value); //Два значения и комбобокс
                    continue;
                }
                if (control.getAttribute("data-role") != null //MaskedTextBox
                    && control.getAttribute("data-role") == "maskedtextbox") {
                    var kendoMasked = $(control).data("kendoMaskedTextBox");
                    if (kendoMasked != undefined) {
                        if (kendoMasked.options.unmaskOnPost == true) {
                            if (kendoMasked.raw() != undefined && kendoMasked.raw() != "")
                                this.append(control.name, kendoMasked.raw());
                            else
                                this.append(control.name, kendoMasked.value());
                        }
                        else
                            this.append(control.name, control.value);
                    }
                    continue;
                }
                if (control.getAttribute("data-role") != null //NumericTextBox
                    && control.getAttribute("data-role") == "numerictextbox") {
                    var kendoNumeric = $(control).data("kendoNumericTextBox");
                    if (kendoNumeric != undefined)
                        this.append(control.name, kendoNumeric.value());
                    continue;
                }
                if (control.getAttribute("data-role") != null //DropDownTree
                    && control.getAttribute("data-role") == "dropdowntree") {
                    var kendoDropDownTree = $(control).data("kendoDropDownTree");
                    if (kendoDropDownTree != undefined && kendoDropDownTree.value() != undefined)
                        this.append(control.name, kendoDropDownTree.value());
                    continue;
                }
                if (control.getAttribute("data-role") != null //ComboBox
                    && control.getAttribute("data-role") == "combobox") {
                    var kendoComboBox = $(control).data("kendoComboBox");
                    if (kendoComboBox != undefined) //&& kendoComboBox.select() != -1
                        this.append(control.name, kendoComboBox.value());
                    continue;
                }
                this.append(control.name, control.value);
            }
        }
        if (editor.editableGridItems.length > 0) { //Редактируемый грид
            for (var i = 0; i < editor.editableGridItems.length; i++) {
                this.append("grid_items[" + i + "][Value]", editor.editableGridItems[i].Value);
            }
            this.append("grid_is_editable", "True");
        }
    };
    //Получение любого считанного input'а по его id
    FormDataNew.prototype.getInputById = function (id) {
        for (var i = 0; i < editor.inputs.length; i++) {
            if (editor.inputs[i].id == id)
                return editor.inputs[i];
        }
        return null;
    };
    //Преобразование formData в строку url
    FormDataNew.prototype.toUrl = function (url) {
        if (url != undefined && url != "") {
            for (var prop in Object.keys(this)) {
                if (this[Object.keys(this)[prop]] != undefined && Object.keys(this)[prop] !== "keys")
                    url = updateQueryStringParameter(url, Object.keys(this)[prop], this[Object.keys(this)[prop]]);
            }
            return url;
        }
    };
    ;
    FormDataNew.prototype.clearFormData = function () {
        for (var prop in this)
            delete this[prop];
        this.keys = new Array();
        this.errorMessage = undefined;
    };
    return FormDataNew;
}());
var formData = new FormDataNew();
var ClickItem = /** @class */ (function () {
    function ClickItem() {
    }
    Object.defineProperty(ClickItem.prototype, "url_action", {
        get: function () {
            if (this._url_action == undefined && vm_Base.url_Action != undefined)
                return vm_Base.url_Action;
            else
                return this._url_action;
        },
        set: function (u) {
            this._url_action = u.replace(new RegExp("amp;", 'g'), "");
            //if (this.keys.indexOf("_url_action") == -1) {
            //    this.keys[this.keys.length] = "_url_action";
            //}
        },
        enumerable: false,
        configurable: true
    });
    Object.defineProperty(ClickItem.prototype, "uri_path", {
        get: function () {
            if (this._uri_path == undefined && vm_Base.uri_path != undefined) {
                this.uri_path = vm_Base.uri_path;
                return vm_Base.uri_path;
            }
            else
                return this._uri_path;
        },
        set: function (u) {
            this._uri_path = u;
            //if (this.keys.indexOf("_uri_path") == -1) {
            //    this.keys[this.keys.length] = "_uri_path";
            //}
        },
        enumerable: false,
        configurable: true
    });
    Object.defineProperty(ClickItem.prototype, "page_guid", {
        get: function () {
            if (this._page_guid == undefined && vm_Base.page_guid != undefined)
                return vm_Base.page_guid;
            else
                return this._page_guid;
        },
        set: function (u) {
            this._page_guid = u;
            //if (this.keys.indexOf("_page_guid") == -1) {
            //    this.keys[this.keys.length] = "_page_guid";
            //}
        },
        enumerable: false,
        configurable: true
    });
    ClickItem.prototype.initialize = function (sender) {
        this.clear();
        var attributes = undefined;
        attributes = sender.attributes;
        if (attributes != undefined) {
            for (var i = 0; i < attributes.length; i++) {
                var attr = attributes[i];
                if (attr.name.trim() == "func_name")
                    this.funcName = attr.value;
                else {
                    clickItem[attr.name] = attr.value;
                    //if (this.keys.indexOf(attr.name) == -1) {
                    //    this.keys[this.keys.length] = attr.name;
                    //}
                }
            }
            this.sender = sender;
        }
        clickItem.fromVmBase = false;
    };
    ClickItem.prototype.initializeFromVmBase = function (sender) {
        this.clear();
        for (var prop in vm_Base) {
            this[String(prop)] = vm_Base[String(prop)];
        }
        this.sender = sender;
        clickItem.fromVmBase = true;
    };
    ClickItem.prototype.click = function () {
        if (this.funcName != undefined)
            eval(this.funcName)();
    };
    ClickItem.prototype.clear = function () {
        for (var prop in this)
            delete this[prop];
        //for (let i = 0; i < this.keys.length; i++)
        //    delete this[this.keys[i]];
        //this.keys = new Array<string>();
        //this.sender = undefined;
        //this.funcName = undefined;
    };
    return ClickItem;
}());
var clickItem = new ClickItem();
function clearClickItem() {
    clickItem = new ClickItem();
}
//# sourceMappingURL=SiteClasses.js.map