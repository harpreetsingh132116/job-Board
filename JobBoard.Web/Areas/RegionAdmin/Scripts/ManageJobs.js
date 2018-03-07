﻿

$(document).ready(function () {
    BindRegions();
    BindTrainings();
    BindJobsList();
    $("#myModal").dialog({
        //bgiframe: true,
        autoOpen: false,
        height: 800,
        width: 500,
        modal: true,
        open: function () {
            jQuery('.ui-widget-overlay').bind('click', function () {
                jQuery('#myModal').dialog('close');
            })
        }
    });

    $('[id$=btnAddNew]').on('click', function () {
        clearControls();
        $("#myModal").dialog('open');
    });
});

//bind Regions Dropdownlist
function BindRegions() {
    $.ajax({
        url: "/RegionAdmin/RegionsMaster/GetAllRegions",
        type: 'get',
        success: function (result) {
            if (result != false) {
                $.each(result, function (data, value) {
                    $("#drpRegion").append($("<option></option>").val(value.Id).html(value.RegionName));
                })
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


function BindTrainings() {

    $.ajax({
        url: "/RegionAdmin/Trainings/GetAllTrainings",
        type: 'get',
        success: function (result) {
            if (result != false) {
                $('#checkboxContainer').empty();
                var content = '';

                $.each(result.Data, function (data1, value1) {
                    content += '<input type="checkbox" name="chk_training" id="' + value1.ID + '">' + value1.Training + '</input>'
                })
                $('#checkboxContainer').html(content);
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


//Bind Academy Teams Grid
function BindJobsList() {
    var url = "/RegionAdmin/ManageJobs/GetAllJobs";

    $("#dvToAcademyTeams").kendoGrid({
        excelExport: function (e) {
            e.workbook.fileName = "result.xlsx";
        }, pdf: {
            allPages: true
        }, toolbar: ["excel", "pdf"],
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
                field: "JobTitle",
                title: "JobTitle"
            },
            {
                field: "IsActive",
                title: "Is Active"
            },
            {
                field: "JobNumber",
                title: "Job Number"
            },
            {
                title: "Edit",
                template: "<a onclick='EditAcademyTeamById(#=ID#)' href='javascript:void(0);'>Edit</a>"
            },
            {
                title: "Delete",
                template: "<a onclick='DeleteJob(#=ID#)' href='javascript:void(0);'>Delete</a>"
            }
        ],
        pageable: {
            pageSizes: [15, 50, 75, 100]
        }
    });
}
function DeleteJob(id) {
    $.ajax({
        url: "/RegionAdmin/ManageJobs/DeleteJob?Id=" + id,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            if (result > 0) {
                if (result == 2) {
                    swal('Job cannot delete, please delete the Job assigned to user.');
                } else if (result == 1) {
                    swal('Job Deleted...!!!');
                    BindJobsList();
                    $("#myModal").dialog('close');
                }
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

function EditAcademyTeamById(Id) {
    clearControls();
    $.ajax({
        url: "/RegionAdmin/ManageJobs/GetJobById?jobId=" + Id,
        type: 'get',
        success: function (result) {
            if (result != false) {
                //$('[id$=hdnTrainingId]').val(result.ID);

                //$('[id$=txtTraining]').val(result.Training);
                $("#hdnJobId").val(result.ID);
                $('#txtJobTitle').val(result.JobTitle);
                $('#txtJobDescription').val(result.JobDescription);
                $('#drpRegion').val(result.RegionId);
                $('#drpStatus').val(result.Status);
                $("#chkIsActive").attr("checked", result.IsActive);


                $('#checkboxContainer input:checkbox').each(function () {
                    if (result.MultipleTrainingsAssignedCommaSeperated != null && result.MultipleTrainingsAssignedCommaSeperated != '') {
                        if (result.MultipleTrainingsAssignedCommaSeperated.split(',').indexOf(this.id) > -1) {
                            $(this).attr('checked', 'checked');
                        }
                    }
                });


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
        var trainings = [];
        $('#dvtraining input:checked').each(function () {
            trainings.push(this.id);
        });


        var model = {
            ID: $("#hdnTrainingId").val()
            , JobTitle: $('#txtJobTitle').val()
            , JobDescription: $('#txtJobDescription').val()
            , RegionId: $('#drpRegion').val()
            , Status: $('#drpStatus').val()
            , IsActive: $("#chkIsActive").attr("checked") ? 1 : 0
            , multipleTrainings: trainings
        };
        var json = JSON.stringify(model);

        $.ajax({
            url: "/RegionAdmin/ManageJobs/InsertUpdateJobs",
            type: 'POST',
            data: json,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                if (result == 99900) {
                    swal('Job Title Already exists, please choose another');
                    
                } else
                    if (result > 0) {
                        BindJobsList();
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

function clearControls() {
    $('[id$=txtJobTitle]').val('');
    $('[id$=txtJobDescription]').val('');
    $('[id$=drpRegion]').val(0);
    $('[id$=drpStatus]').val(0);
    $('#checkboxContainer').find('input[type=checkbox]:checked').removeAttr('checked');

    $('[id$=sptxtJobTitle]').css('display', 'none');
    $('[id$=sptxtJobDescription]').css('display', 'none');
    $('[id$=spdrpRegion]').css('display', 'none');
    $('[id$=spchk_training]').css('display', 'none');
    $('[id$=spdrpStatus]').css('display', 'none');
    $("#hdnJobId").val(0);
}

function validate() {
    var result = true;

    if ($('[id$=txtJobTitle]').val() == '') {
        $('[id$=sptxtJobTitle]').css('display', 'block');
        result = false;
    } else {
        $('[id$=sptxtJobTitle]').css('display', 'none');
    }

    if ($('[id$=txtJobDescription]').val() == '') {
        $('[id$=sptxtJobDescription]').css('display', 'block');
        result = false;
    } else {
        $('[id$=sptxtJobDescription]').css('display', 'none');
    }


    if ($('[id$=drpRegion]').val() == 0) {
        $('[id$=spdrpRegion]').css('display', 'block');
        result = false;
    } else {
        $('[id$=spdrpRegion]').css('display', 'none');
    }

    //if ($('input[name=chk_training]:checked').length > 0) {
    //    $('[id$=spchk_training]').css('display', 'none');
    //} else {
    //    $('[id$=spchk_training]').css('display', 'block');
    //    result = false;
    //}

    if ($('[id$=drpStatus]').val() == 0) {
        $('[id$=spdrpStatus]').css('display', 'block');
        result = false;
    } else {
        $('[id$=spdrpStatus]').css('display', 'none');
    }


    return result;
}
