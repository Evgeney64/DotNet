// **************************************************************************************************
//var formData = {};  //Глобальный объект, аналог базовой form. Нужен для удобства отладки, в отличие от обычной FormData виден на клиенте
var currControl = {};
//var vm_Base = {};
var url = "";
var editor1 = {  //Глобальный объект, который содержит в себе все измененные input'ы, проверенные input'ы и прочее
    inputs: [], //Все input'ы
    inputsChanged: [],  //Только измененные input'ы
    inputsIsRequired: [],   //input'ы, которые необходимо заполнить
    editableGridItems: [],  //input'ы редактируемого грида
    errorMessage: "",   //сообщение об ошибке после проверки
    //Считываение всех input'ов и заполнение соответсвующих массивов
    readInputs: function (formName, group) {    
        this.inputs = [];
        this.inputsChanged = [];
        this.inputsIsRequired = [];
        this.editableGridItems = [];
        formData = createNewFormData();     //Каждый раз при вызове объект создается заново

        if (document.forms[formName] == undefined)
            return;
        //Возвращение всех элементов формы по заданному селектру
        var all_inputs = document.forms[formName].querySelectorAll("input.ctrl-input[id], textarea.ctrl-input[id], select.ctrl-input[id], li.ctrl-input input, span.ctrl-input[id]");
        if (group != undefined)
            all_inputs = document.forms[formName].querySelectorAll("input.ctrl-input[group='" + group + "']");

        for (i = 0; i < all_inputs.length; i++) {
            this.inputs[this.inputs.length] = all_inputs[i];
            if (all_inputs[i].getAttribute("Changed") != null)
                this.inputsChanged[this.inputsChanged.length] = all_inputs[i];
            if (all_inputs[i].getAttribute("isrequired") != null)
                this.inputsIsRequired[this.inputsIsRequired.length] = all_inputs[i];
        }
        var editable_grids = document.forms[formName].getElementsByClassName("grid k-editable");
        for (i = 0; i < editable_grids.length; i++) {
            if (editable_grids[i].getAttribute("grid_is_editable").length > 0) {
                entity_ReadData_EditableGrid(editable_grids[i], this.editableGridItems);
            }
        }
        var editor_grids = document.forms[formName].getElementsByClassName("grid k-reorderable");
        for (i = 0; i < editor_grids.length; i++) {
            if (editor_grids[i].getAttribute("name") != null
                && editor_grids[i].getAttribute("name").length > 0
                && getRowId_ByGuid(editor_grids[i].getAttribute("page_guid")) != null
                ) {
                editor_grids[i].name = editor_grids[i].getAttribute("name");
                editor_grids[i].value = getRowId_ByGuid(editor_grids[i].getAttribute("page_guid"));
                this.inputsChanged[this.inputsChanged.length] = editor_grids[i];
            }
        }
        //Проверяем все input'ы
        this.checkRequred();
        //Формируем formData из input'ов 
        this.to_formData();
    },
    //Проверка массивов обязательных и измененных input'ов. Результатом выполнения будет errorMessage
    checkRequred: function () { 
        this.errorMessage = "";
        if (this.inputsIsRequired.length > 0) { //Проверка на заполнение обязательных input'ов
            for (i = 0; i < this.inputsIsRequired.length; i++) {
                var control = this.inputsIsRequired[i];
                // ($Роев)
                if (control.getAttribute("controltype") != "Upload" && control.getAttribute("controltype") != "Selector") {
                    if (control.value == null || control.value == "")
                        this.errorMessage += "<div>Не задано поле: &nbsp[" + control.title + "]</div>";
                }
                if (control.getAttribute("controltype") == "Upload") {
                    if ($(control).data("kendoUpload").getFiles().length == 0)
                        this.errorMessage += "<div>Не загружены файлы для поля: " + control.getAttribute("title") + "</div>";
                }
                if (control.getAttribute("controltype") == "Selector") {
                    var selector_id = $("#val_" + control.getAttribute("page_guid")).attr("selector_id");
                    if (selector_id == undefined) {
                        this.errorMessage += "<div>Не задано поле: &nbsp[" + control.title + "]</div>";
                    }
                }
                //if ((control.value == null || control.value == "") && control.getAttribute("controltype") != "Upload")
                //    this.errorMessage += "<div>Не задано поле: &nbsp[" + control.title + "]</div>";
                //if (control.getAttribute("controltype") == "Upload" && $(control).data("kendoUpload").getFiles().length == 0)
                //    this.errorMessage += "<div>Не загружены файлы для поля: " + control.getAttribute("title") + "</div>";
            }
        }
        if (this.inputsChanged.length > 0) {    //Остальные проверки
            for (i = 0; i < this.inputsChanged.length; i++) {
                //Проверки файлов
                var control = this.inputsChanged[i];
                if (control.getAttribute("controltype") == "Upload") {
                    var files = $(control).data("kendoUpload").getFiles();
                    if (files.length != 0) {
                        //проверяем количество
                        if (files.length > 20) {
                            this.errorMessage += "<div>Не более 20 файлов!</div>"
                        }
                        // проверяем размер
                        for (var j = 0; j < files.length; j++) {
                            if (files[j].rawFile.size > 71680000) {
                                this.errorMessage += "<div>Файлы не более 70 мегабайт!</div>"
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
    },
    //Заполнение глобальной formData. Будет видна везде в рамках вызванного window, может дополняться обычными переменными через метод append()
    to_formData: function () {   
        if (this.inputsChanged.length > 0) {
            for (i = 0; i < this.inputsChanged.length; i++) {
                var control = this.inputsChanged[i];
                if (control.type == "file") {   //fileUpload
                    var files = $(control).data("kendoUpload").getFiles();
                    for (var j = 0; j < files.length; j++) {
                        formData.append(control.name + ".(" + j + ")", files[j].rawFile);
                    }
                    formData.append($(control).data("kendoUpload").name, control.getAttribute("name"));
                    continue;
                }
                if (control.type == "checkbox") {   //Чекбоксы
                    if (control.getAttribute("iscurrentdatecheckbox") == "1") {   //Чекбокс "Действующие"
                        var check_control = $("#check_" + control.getAttribute("page_guid"))[0];
                        if (check_control == undefined)
                            continue;
                        if (check_control.checked)
                            formData.append(control.name, "on");
                        else
                            formData.append(control.name, "off");
                        continue
                    }
                    else {
                        if (control.checked)  //Обычные
                            formData.append(control.name, "on");
                        else
                            formData.append(control.name, "off");
                        continue
                    }
                }
                if (control.type == "select-multiple") {    //МультиКомбоБокс
                    formData.append(control.name, $(control).data("kendoMultiSelect").value());
                    continue
                }
                if (control.getAttribute("istext") != null     //текст
                    && control.getAttribute("istext").length > 0
                    && control.getAttribute("istext") == "True") {
                    formData.append(control.getAttribute("name"), control.innerText);
                    continue
                }
                if (control.getAttribute("isdaterange") != null     //Диапазон дат
                    && control.getAttribute("isdaterange").length > 0
                    && control.getAttribute("isdaterange") == "True") {
                    if (control.getAttribute("idvalend") != null) {
                        formData.append(control.name, control.value + ";" + this.getInputById(control.getAttribute("idvalend")).value); //Значения двух дат объединяем в одно
                    }
                    continue
                }
                if (control.getAttribute("idvalend") != null     //Числовой диапазон
                    && control.getAttribute("idcombobox") != null) {
                    formData.append(control.name, control.value + ";" + this.getInputById(control.getAttribute("idvalend")).value + ";" + this.getInputById(control.getAttribute("idcombobox")).value); //Два значения и комбобокс
                    continue;
                }
                if (control.getAttribute("data-role") != null   //MaskedTextBox
                    && control.getAttribute("data-role") == "maskedtextbox") {
                    var kendoMasked = $(control).data("kendoMaskedTextBox");
                    if (kendoMasked != undefined) {
                        if (kendoMasked.options.unmaskOnPost == true)
                            formData.append(control.name, kendoMasked.raw());
                        else
                            formData.append(control.name, control.value);
                    }
                    continue;
                }
                if (control.getAttribute("data-role") != null   //NumericTextBox
                    && control.getAttribute("data-role") == "numerictextbox") {
                    var kendoNumeric = $(control).data("kendoNumericTextBox");
                    if (kendoNumeric != undefined)
                        formData.append(control.name, kendoNumeric.value());
                    continue;
                }
                if (control.getAttribute("data-role") != null   //DropDownTree
                    && control.getAttribute("data-role") == "dropdowntree") {
                    var kendoDropDownTree = $(control).data("kendoDropDownTree");
                    if (kendoDropDownTree != undefined && kendoDropDownTree.value() != undefined)
                        formData.append(control.name, kendoDropDownTree.value());
                    continue;
                }
                if (control.getAttribute("data-role") != null   //ComboBox
                    && control.getAttribute("data-role") == "combobox") {
                    var kendoComboBox = $(control).data("kendoComboBox");
                    if (kendoComboBox != undefined)  //&& kendoComboBox.select() != -1
                        formData.append(control.name, kendoComboBox.value());
                    continue;
                }
                formData.append(control.name, control.value);
            }
        }
        if (this.editableGridItems.length > 0) {    //Редактируемый грид
            for (var i = 0; i < this.editableGridItems.length; i++) {
                formData.append("grid_items[" + i + "][Value]", this.editableGridItems[i].Value);
            }
            formData.append("grid_is_editable", "True");
        }
    },
    //Заполнение глобальной переменной url. Тоже может передаваться в контроллер (пока не используется!)
    toUrl: function (url) {     
        if (url != undefined && url != "") {

            //if (this.inputsChanged.length > 0) {
            //    for (i = 0; i < this.inputsChanged.length; i++) {
            //        var control = this.inputsChanged[i];
            //        url = updateQueryStringParameter(url, control.name, control.value);
            //    }
            //}

            if (formData.keys.length > 0)
            {
                for (i = 0; i < formData.keys.length; i++) {
                    url = updateQueryStringParameter(url, formData.keys[i], formData[formData.keys[i]]);
                }
            }
            return url;
        }
    },
    //Получение любого считанного input'а по его id
    getInputById: function (id) {   
        for (i = 0; i < this.inputs.length; i++) {
            if (this.inputs[i].id == id)
                return this.inputs[i];
        }
        return null;
    }
}
//Функция инициализации всех параметров и переменных с сервера. Создается глобальный объект vm_Base, у которого свойствами будут все переданные параметры
function initialize(arguments, not_rewrite) {
    if (!not_rewrite || not_rewrite == undefined)
        vm_Base = {};
    var func_str = arguments.callee.toString(); //Получаем исходный код функции в виде строки
    var args = func_str.split("(")[1].split(")")[0].trim().split(",");  //Берем оттуда только список пришедших аргументов
    for (i = 0; i < args.length; i++) {
        if (vm_Base[args[i].trim()] != undefined && vm_Base[args[i].trim()] != "" && (arguments[i] === "" || arguments[i] == undefined))
            continue;
        if (args[i].trim() != "" && args[i].trim() != undefined)    //Все остальные аргументы функции преобразуем в свойства объекта vm_Base
            vm_Base[args[i].trim()] = arguments[i];
        if (args[i].trim() == "vm_Base_params" && arguments[i] != undefined) {  //Парсим строку vm_Base_params со свойствами vm_Base с сервера и преобразуем в свойства нового объекта vm_Base
            var params = arguments[i].split("|");
            for (j = 0; j < params.length - 1; j++) {
                if (vm_Base[params[j].split(":")[0]] == undefined || vm_Base[params[j].split(":")[0]] != params[j].split(":")[1])
                    vm_Base[params[j].split(":")[0]] = params[j].split(":")[1];
            }
            continue;
        }
        if (args[i].trim() == "control_params" && arguments[i] != undefined) {  //Парсим строку со свойствами контрола (пока не используется)
            var params = arguments[i].split("|");
            for (j = 0; j < params.length - 1; j++) {
                currControl[params[j].split(":")[0]] = params[j].split(":")[1];
            }
            continue;
        }
    }
    if (vm_Base.url_Action != undefined && Top().isAutoTestClient == true) {
        vm_Base.url_Action = addToUrl(vm_Base.url_Action, "isAutoTest", true);
        if (check_AutoTest_Logging() == true) {
            vm_Base.is_AutoTest_Logging = true;
            //Не получается передавать сразу в updateQueryStringParameter функцию check_AutoTest_Logging(). Для true/false не работает
            vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "isAutoTestLogging", 'true');
        }
        else {
            vm_Base.is_AutoTest_Logging = false;
            vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "isAutoTestLogging", 'false');
        }
    }
    if (vm_Base.url_Content == undefined && Top().vm_Base.url_Content != undefined)
        vm_Base.url_Content = Top().vm_Base.url_Content;
}
//Функция которая создает новую formData
function createNewFormData() {
    var formData = {
        keys: [],   //Список всех ключей
        //Аналог функции FormData().append()
        append: function (name, value) {
            if (this.keys.indexOf(name) == -1) {
                this.keys[this.keys.length] = name;
            }
            this[name] = value;
        },
        //Функция для перевода formData в FormData(), которую потом передаем в контроллер. В контроллере она будет доступна в Request.Form
        toFormData: function () {
            var formData = new FormData();
            for (i = 0; i < this.keys.length; i++)
                formData.append(this.keys[i], this[this.keys[i]]);
            return formData;
        }
    }
    return formData;
}
