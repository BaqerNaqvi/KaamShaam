
function ValidateByClassName(className) {
    var requiredControls = $('.' + className);
    //now find required fields
    var counter = 0;
    for (var i = 0; i < requiredControls.length; i++) {
        var requiredField = $(requiredControls[i]);
        var fieldvalue = requiredField.val().trim();
        var fieldType = requiredField.attr('type');
        var isError = false;
        switch (fieldType) {
            case "email":
                isError = !validateEmail(requiredField);
                break;
        }
        if (fieldvalue === null || fieldvalue === undefined || (fieldvalue === "" || (fieldvalue.length >= requiredField.data('length'))) || isError) {
            counter++;
            requiredField.addClass('input-validation-error');
        } else {
            requiredField.removeClass('input-validation-error');
        }
    }
    if (counter > 0) {
        //implementing focus back to error
        var divId = $('.' + className + ".input-validation-error")[0].id;
        if ($("#" + divId).length > 0)
            $("#" + divId).focus();
        return false;
    }
    return true;
}

function validateEmail(email) {

    var oEmail = $(email);
    var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
    if (!emailReg.test(oEmail.val())) {
        //oEmail.val("");
        //oEmail.attr("placeholder", "Enter valid email.");
        return false;
    } else {
        return true;
    }
}


function hideModel(modelId) {
    $('#' + modelId).modal('hide');
}