var HttpRequest = {

    /* @description Ajax function
    * @param {method} GET,POST,PUT.
    * @param {url} /Area/Controller/Action
    * @param {data} Parameter
    * @param {successCallBack}  Success inner Function
    * @param {errorCallBack}  Error inner Function
    * @param {loader}  Ajax loader
    */
    Ajax: function (method, url, data, successCallBack, errorCallBack, loader = false) {
       
        
        let ajaxObj = $.ajax({
            type: method.toUpperCase(),
            url: url,
            data: data,
            dataType: 'json',
            async: false,
            cache: false,
            beforeSend: function () {
                
                if (loader == true) {
                    $("body").addClass("loading");
                }
            },
            success: successCallBack,
            error: function (err, type, httpStatus) {
                if (errorCallBack) {
                    errorCallBack;
                } else {
                    HttpRequest.FailureCallback(err, type, httpStatus);
                }
                if (loader == true) {
                    $("body").removeClass("loading");

                }
            },
            complete: function () {
               
                if (loader === true) {
                    $("body").removeClass("loading");
                    $(".overlay").hide();
                }
            }
        });
        
        return ajaxObj;
    },
    AjaxAsync: function (method, url, data, successCallBack,disableBtnID, errorCallBack, loader = false) {


        let ajaxObj = $.ajax({
            type: method.toUpperCase(),
            url: url,
            data: data,
            dataType: 'json',
            async: true,
            cache: false,
            beforeSend: function () {
                if (disableBtnID) {
                    $(disableBtnID).prop('disabled', true);
                }
                if (loader == true) {
                    $("body").addClass("loading");
                }
            },
            success: successCallBack,
            error: function (err, type, httpStatus) {
                if (disableBtnID) {
                    $(disableBtnID).prop('disabled', false);
                }
                if (errorCallBack) {
                    errorCallBack;
                } else {
                    HttpRequest.FailureCallback(err, type, httpStatus);
                }
                if (loader == true) {
                    $("body").removeClass("loading");

                }
            },
            complete: function () {
                if (disableBtnID) {
                    $(disableBtnID).prop('disabled', false);
                }
                if (loader === true) {
                    $("body").removeClass("loading");
                    $(".overlay").hide();
                }
            }
        });

        return ajaxObj;
    },
    AjaxData: function (method, url, data,  loader = false) {
    
        let returnData = "";
        $.ajax({
            beforeSend: function () {
                if (loader === true) {
                    $("body").addClass("loading");
                    $(".overlay").show();
                }
            },
            type: method.toUpperCase(),
            url: url,
            data: data,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                returnData = data;
            },
            error: function (err, type, httpStatus) {
                HttpRequest.FailureCallback(err, type, httpStatus);
            },
            complete(xhr, status) {
                if (loader == true) {
                    $("body").removeClass("loading");
                    $(".overlay").hide();
                }
            }
        });
        return returnData;
    },
    AjaxDataAsync: function (method, url, data, loader = false) {
        var promise = new Promise(function (resolve, reject) {
            $.ajax({
                beforeSend: function () {
                    if (loader === true) {
                        $("body").addClass("loading");
                    }
                },
                type: method.toUpperCase(),
                url: url,
                data: data,
                dataType: 'json',
                success: function (data) {
                    resolve(data);
                },
                error: function (jqXHR) {
                    reject(jqXHR.status);
                },
                complete(xhr, status) {
                    if (loader == true) {
                        $("body").removeClass("loading");
                        $(".overlay").hide();
                    }
                }
            });
        });
        let returnData = "";
        promise.then(function (res) {
            returnData = res;
        });
        promise.catch(function (status) {
            $.alert.open('error', "Error", "Find error on load.");
        });
        return returnData;
    },

    /* @description Ajax function.
    * @param {method} GET,POST,PUT.
    * @param {url} /Area/Controller/Action
    * @param {data} Parameter
    * @param {targetFieldId}   Selected Dropdown Field
    * @param {loader}  Ajax loader
    */
    DropDown: function (method, url, data, targetFieldId, loader = false) {

        let ddlTargetField = $("#" + targetFieldId);
        if (loader == true) {
            $("#loadingImage").show();
        }
        let ajaxObj = $.ajax({
            type: method.toUpperCase(),
            beforeSend: function () {
                if (loader == true) {
                    $("body").addClass("loading");
                }
            },
            url: url,
            data: data,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {

                ddlTargetField.html('');
                var _options = "";
                $.each(data, function (id, option) {
                    _options += '<option value="' + option.Value + '">' + option.Text + '</option>';
                });
                ddlTargetField.append(_options);
                if (loader == true) {
                    $("#loadingImage").hide();
                }

            },
            error: function (err, type, httpStatus) {
                if (loader == true) {
                    $("#loadingImage").hide();
                }
                HttpRequest.FailureCallback(err, type, httpStatus);
            },
            complete(xhr, status) {
                if (loader == true) {
                    $("body").removeClass("loading");
                }
            }

        });
        return ajaxObj;
    },

    DropDownCustom: function (method, url, data, targetFieldId, loader = false) {
        let ddlTargetField = $("#" + targetFieldId);
        if (loader == true) {
            $("#loadingImage").show();
        }
        let ajaxObj = $.ajax({
            type: method.toUpperCase(),
            url: url,
            data: data,
            dataType: 'json',
            async: false,
            cache: false,
            beforeSend: function () {
                if (loader == true) {
                    $("body").addClass("loading");
                }
            },
            success: function (data) {
               
                ddlTargetField.html('');
                var _options = "";
                $.each(data, function (id, option) {
                    _options += `<option data-Custom1="${option.Custom1}" data-Custom2="${option.Custom2}" data-Custom3="${option.Custom3}" data-Custom4="${option.Custom4}" data-Custom5="${option.Custom5}" data-Custom6="${option.Custom6}" value="${option.Value}">${option.Text}</option>`;
                });
                ddlTargetField.append(_options);
                if (loader == true) {
                    $("#loadingImage").hide();
                }

            },
            error: function (err, type, httpStatus) {
                if (loader == true) {
                    $("#loadingImage").hide();
                }
                HttpRequest.FailureCallback(err, type, httpStatus);
            },
            complete(xhr, status) {
                if (loader == true) {
                    $("body").removeClass("loading");
                }
            }
        });
        return ajaxObj;

    },

    DropDownOptions: function (method, url, data) {

        let options = "";
        $.ajax({
            type: method.toUpperCase(),
            url: url,
            data: data,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {

                $.each(data, function (id, option) {
                    if (option.Selected === false) {
                        selectedItem = "";
                    } else {
                        selectedItem = 'selected="selected"';
                    }
                    options = options + '<option value="' + option.Value + '" ' + selectedItem + ' >' + option.Text + '</option>';
                });

            },
            error: function (err, type, httpStatus) {
                HttpRequest.FailureCallback(err, type, httpStatus);
            }
        });

        return options;
    },
    DropDownCustomOptions: function (method, url, data, ) {
        var _options = "";
         $.ajax({
            type: method.toUpperCase(),
            url: url,
            data: data,
            dataType: 'json',
            async: false,
            cache: false,
            
            success: function (data) {
             
                $.each(data, function (id, option) {
                    _options += `<option data-Custom1="${option.Custom1}" data-Custom2="${option.Custom2}" data-Custom3="${option.Custom3}" data-Custom4="${option.Custom4}" data-Custom5="${option.Custom5}" value="${option.Value}">${option.Text}</option>`;
                });
            },
            error: function (err, type, httpStatus) {
                HttpRequest.FailureCallback(err, type, httpStatus);
            }
        });
        return _options;

    },

    DropDownDefault: function (targetFieldId) {
        let ddlTargetField = $("#" + targetFieldId);
        ddlTargetField.html('<option value="">Please Select</option>');
    },

    FailureCallback: function (err, type, httpStatus) {
        debugger;
        var result = 0;
        var title = "";
        var failureMessage = "";
        console.log(err);


        debugger;
        let errorList = err.responseJSON;
        if (errorList != undefined && (errorList.status == 460 || errorList.status == 461)) {
            let sl = 0;
            title = "System found error.";
            $.each(errorList.errors, function (i, v) {
                failureMessage += `Error ${++sl}: <span class='text-danger'>${v}</span> <br/>`;
            });
        }
        else if (err.status == 400) {
            title = err.statusText;
            failureMessage = err.statusText;
        }
        else if (err && type != null) {
            title = "System found error.";
            failureMessage=`Status:${err.statusText} & Code : ${err.status}`;
        }
        else if (err.responseJSON.result == 99) {

            title = err.responseJSON.message;
            if (err.responseJSON.Errors.length > 0) {
                $.each(err.responseJSON.Errors, function (i, v) {
                    failureMessage += `<b>${v.PropertyName}:</b> ${v.ErrorMessage} <br/>`;
                });
            }
           failureMessage=failureMessage;

        }
        else {
            title = "System found error.";
            failureMessage = 'Error occurred in ajax call' + err.status + " - " + err.responseText + " - " + httpStatus;
        }
        $.alert.open('error', title, failureMessage);

        console.log(failureMessage);
    },

    Select2GroupData: function (data) {
        var singleObject = {
            "text": "",
            "children": [
                {
                    "id": "",
                    "text": ""
                }
            ]
        };

        if (data == null) {
            return {
                "text": "",
                "children": [{}]
            }
        }

        const result = data.reduce((gdata, d) => {
            if (d.Value != "") {
                const found = gdata.find(a => a.text === d.Group.Name);
                const value = { id: d.Value, text: d.Text };
                if (found) {
                    found.children.push(value);
                }
                else {
                    gdata.push({ text: d.Group.Name, children: [{ id: d.Value, text: d.Text }] });
                }
            }
            return gdata;
        }, []);

        return result;

    },
   
    Select2NormalData: function (data,) {
        var singleObject = {
            "text": "",
            "children": [
                {
                    "id": "",
                    "text": ""
                }
            ]
        };

        if (data == null) {
            return {
                "text": "",
                "id": ""
            }
        }

    },

    DropDownSelect2: function (method, url, paramData, targetFieldId, loader = false) {
        let ddlTargetField = $("#" + targetFieldId);
        ddlTargetField.html("");
        ddlTargetField.select2("destroy");
  
        ddlTargetField.select2({
            // minimumInputLength: 0,
            width:"98%",
            placeholder: 'Please Select',
            allowClear: true,
            ajax: {
                url: url,
                dataType: 'json',
                delay: 250,
                beforeSend: function () {
                    if (loader == true) {
                        $("body").addClass("loading");
                    }
                },
                type: method.toUpperCase(),
                data: function (params) {
                    paramData['predict'] = params.term;
                    return paramData;
                },
                complete(xhr, status) {
                    if (loader == true) {
                        $("body").removeClass("loading");
                    }
                },
                processResults: function (data, params) {
                    return {
                        results: $.map(data, function (item) {
                            return { id: item.Value, text: item.Text };
                        })
                    }
                }
            }
        })
    },

    DropDownSelect2Group: function (method, url, paramData, targetFieldId, loader = false) {

        let ddlTargetField = $("#" + targetFieldId);
        ddlTargetField.html("");
        ddlTargetField.select2("destroy");

        ddlTargetField.select2({
            placeholder: 'Please Select',
            allowClear: true,
            width: "98%",
            minimumInputLength: 0,
            ajax: {
                url: url,
                dataType: 'json',
                delay: 250,
                type: method.toUpperCase(),
                beforeSend: function () {
                    if (loader == true) {
                        $("body").addClass("loading");
                    }
                },
                data: function (params) {

                    paramData['predict'] = params.term;
                    return paramData;
                },
                complete(xhr, status) {
                    if (loader == true) {
                        $("body").removeClass("loading");
                    }
                },
                processResults: function (data, params) {
                    var res = HttpRequest.Select2GroupData(data);
                    return {
                        results: res
                    }
                }
            }
        })
    },
    //SearchTableRowData: function (that) {
    //    var input, filter, tableId, table, tr, td, i, txtValue;
    //    input = that;
    //    filter = input.value.trim().toUpperCase();
    //    tableId = that.closest('table').id;
    //    tr = $('#' + tableId).find('tbody tr');
    //    for (i = 0; i < tr.length; i++) {
    //        td = tr[i].getElementsByTagName("td")[0];
    //        if (td) {
    //            txtValue = td.textContent.trim() || td.innerText.trim();
    //            if (txtValue.toUpperCase().indexOf(filter) > -1) {
    //                tr[i].style.display = "";
    //            } else {
    //                tr[i].style.display = "none";
    //            }
    //        }
    //    }
    //}
}