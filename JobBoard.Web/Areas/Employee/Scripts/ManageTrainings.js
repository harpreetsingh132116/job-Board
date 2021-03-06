﻿

$(document).ready(function () {

    BindTrainingsList();
    $("#myModal").dialog({
        bgiframe: true,
        autoOpen: false,
        height: 500,
        width: 500,
        modal: true,
        open: function () {
            jQuery('.ui-widget-overlay').bind('click', function () {
                jQuery('#myModal').dialog('close');
            })
        }
    });

    $('[id$=btnAddNew]').on('click', function () {
        $("#myModal").dialog('open');
    });
});

//Bind Academy Teams Grid
function BindTrainingsList() {
    var url = "/Employee/Trainings/GetAllTrainings";

    $("#dvToAcademyTeams").kendoGrid({
        dataSource: {
            autoBind: false,
            transport: {
                read: url
            },
            schema: {
                data: "Data",
                total: "Total",
                errors: "Errors",
                model: {
                    fields: {
                        OrgRule: { type: "string" }
                    }
                }
            },
            pageSize: 15,
            serverPaging: false,
        },
        pageable: {
            pageSize: 15,
            info: true,
            refresh: false
        },
        columnMenu: true,
        onetouch: true,
        sortable: true,
        scrollable: true,
        filterable: {
            extra: false
        },
        columns: [
            {
                field: "Training",
                title: "Training Name"
            },
            {
                field: "IsActive",
                title: "Is Active"
            },
            {
                title: "Edit",
                template: "<a onclick='EditAcademyTeamById(#=ID#)' href='javascript:void(0);'>Edit</a>"
            }
        ],
        pageable: {
            pageSizes: [15, 50, 75, 100]
        }
    });
}


function EditAcademyTeamById(Id) {
    $.ajax({
        url: "/Employee/Trainings/GetTrainingsById?Id=" + Id,
        type: 'get',
        success: function (result) {
            if (result != false) {
                $('[id$=hdnTrainingId]').val(result.ID);

                $('[id$=txtTraining]').val(result.Training);

                $("#myModal").dialog('open');
            }
            else {
                alert("Some error occured. Please try again.");
            }
        },
        failure: function (result) {
            swal(result.responseText);
        },
        error: function (result) {
            swal(result.responseText);
        }
    });
}


function UpdateAcademyTeamById() {

    if (validate()) {

        var model = { ID: $("#hdnTrainingId").val(), Training: $('#txtTraining').val() };
        var json = JSON.stringify(model);



        // var model = { ID: $("#hdnTrainingId").val(), Training: $('#txtTraining').val() };
        // var json = JSON.stringify(model);

        $.ajax({
            url: "/Employee/Trainings/InsertUpdateTrainings",
            type: 'POST',
            data: json,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                if (result > 0) {
                    BindTrainingsList();
                    swal('Good Job...!!!');

                    $("#myModal").dialog('close');
                }
                else {
                    alert("Some error occured. Please try again.");
                }
            },
            failure: function (result) {
                swal(result.responseText);
            },
            error: function (result) {
                swal(result.responseText);
            }
        });
    }
}




function validate() {
    var result = true;

    if ($('[id$=txtTraining]').val().trim() == '') {
        $('[id$=sptxtTraining]').css('display', 'block');
        result = false;
    } else {
        $('[id$=sptxtRegion]').css('display', 'none');
    }

    return result;
}