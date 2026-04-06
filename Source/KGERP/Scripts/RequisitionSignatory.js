// APR Signatory Approval System - JavaScript
(function () {
    'use strict';

    var signatoryTable;
    var currentIntegrateWith = '';
    var currentEmployeeId = null;
    var currentCompanyId = null;
    var currentDepartmentId = null;
    var currentDesignationId = null;
    var deleteId = 0;
    var isTableInitialized = false;
    var companyId = 0;

    $(document).ready(function () {
        companyId = $('#hdnCompanyId').val();
        initializeComponents();  // UNCOMMENTED - This is required!
        bindEvents();
        console.log('initialized with companyId:', companyId);
    });

    function initializeComponents() {
        // Initialize DataTable (but don't load data initially)
        initializeDataTable();

        // Initialize Employee Autocomplete
        initializeEmployeeAutocomplete();

        // Set initial module name if available
        currentIntegrateWith = $('#ddlModuleName').val();
    }

    function bindEvents() {
        // Module Name change
        $('#ddlModuleName').on('change', function () {
            currentIntegrateWith = $(this).val();
            reloadDataTable();  // REMOVED conditional - always reload
        });

        // Employee change
        $('#ddlEmployee').on('change', function () {
            currentEmployeeId = $(this).val();
            reloadDataTable();  // REMOVED conditional - always reload
        });

        // Cascading Dropdowns
        $('#ddlCompany').on('change', function () {
            currentCompanyId = $(this).val();
            loadDepartments(currentCompanyId);
            reloadDataTable();  // REMOVED conditional - always reload
        });

        $('#ddlDepartment').on('change', function () {
            debugger
            currentDepartmentId = $(this).val();
            loadDesignations(currentDepartmentId);
            reloadDataTable();
        });

        $('#ddlDesignation').on('change', function () {
            currentDesignationId = $(this).val();
            reloadDataTable();  // REMOVED conditional - always reload
        });

        // Add New button
        $('#btnAddNew').on('click', function () {
            openCreateEditModal(0);
        });

        // Replace Approver button
        $('#btnReplaceApprover').on('click', function () {
            openReplaceApproverModal();
        });

        // Delete confirmation
        $('#btnConfirmDelete').on('click', function () {
            deleteSignatory();
        });
    }

    function initializeDataTable() {
        signatoryTable = $('#tblSignatories').DataTable({
            processing: false,
            serverSide: true,
            ajax: {
                url: '/RequisitionSignatory/GetSignatoryData',
                type: 'POST',
                data: function (d) {
                    d.integrateWith = currentIntegrateWith;
                    d.companyId = currentCompanyId;
                    d.employeeId = currentEmployeeId;
                    d.departmentId = currentDepartmentId;
                    d.designationId = currentDesignationId;
                }
            },

            columns: [
                { data: 'employeeName', name: 'employeeName' },
                { data: 'companyName', name: 'companyName' },
                { data: 'departmentName', name: 'departmentName' },
                { data: 'designationName', name: 'designationName' },
                {
                    data: 'approverName',
                    name: 'approverName',
                    render: function (data, type, row) {
                        return row.approverCode + ' - ' + data;
                    }
                },
                { data: 'level', name: 'level' },
                {
                    data: 'requisitionSignatoryId',
                    orderable: false,
                    width: '8%',
                    render: function (data, type, row) {
                        return '<a href="javascript:void(0);" onclick="editSignatory(' + data + ')" class="btn btn-xs btn-primary" title="Edit">' +
                            '<i class="fa fa-edit"></i></a> ' +
                            '<a href="javascript:void(0);" onclick="confirmDelete(' + data + ')" class="btn btn-xs btn-danger" title="Delete">' +
                            '<i class="fa fa-trash"></i></a>';
                    }
                }
            ],
            order: [[4, 'asc']],
            pageLength: 10,
            lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]]
        });
    }

    function reloadDataTable() {
        debugger
        if (!isTableInitialized) {
            // First time loading - just mark as initialized and load
            isTableInitialized = true;
        }

        if (signatoryTable) {
            signatoryTable.ajax.reload();
        }
    }

    // Cascading Dropdown Functions
    function loadDepartments(companyId) {
        $('#ddlDepartment').html('<option value="">-- Select Department --</option>');
        $('#ddlDesignation').html('<option value="">-- Select Designation --</option>');

        if (!companyId) return;

        $.ajax({
            url: '/RequisitionSignatory/GetDepartments',
            type: 'POST',
            data: { companyId: companyId },
            success: function (data) {
                $.each(data, function (i, item) {
                    $('#ddlDepartment').append($('<option></option>').val(item.Value).html(item.Text));
                });
            },
            error: function () {
                showNotification('Error loading departments', 'error');
            }
        });
    }

    function loadDesignations(departmentId) {
        $('#ddlDesignation').html('<option value="">-- Select Designation --</option>');

        if (!departmentId) return;

        $.ajax({
            url: '/RequisitionSignatory/GetDesignations',
            type: 'POST',
            data: { departmentId: departmentId },
            success: function (data) {
                $.each(data, function (i, item) {
                    $('#ddlDesignation').append($('<option></option>').val(item.Value).html(item.Text));
                });
            },
            error: function () {
                showNotification('Error loading designations', 'error');
            }
        });
    }

    // Employee Autocomplete Initialization using jQuery UI
    function initializeEmployeeAutocomplete() {
        // Convert dropdown to autocomplete textbox
        var $employeeDropdown = $('#ddlEmployee');
        var $employeeInput = $('<input type="text" id="txtEmployee" class="form-control" placeholder="Type employee name or code..." />');
        var $employeeHidden = $('<input type="hidden" id="hdnEmployeeId" />');

        $employeeDropdown.replaceWith($employeeInput);
        $employeeInput.after($employeeHidden);

        $employeeInput.autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '/RequisitionSignatory/EmployeeAutoComplete',
                    type: 'POST',
                    dataType: 'json',
                    data: { prefix: request.term, companyId:companyId },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return {
                                label: item.Name,
                                value: item.Name,
                                id: item.Id
                            };
                        }));
                    }
                });
            },
            minLength: 2,
            select: function (event, ui) {
                $employeeHidden.val(ui.item.id);
                currentEmployeeId = ui.item.id;
                // Reset Company, Department, and Designation dropdowns
                $('#ddlCompany').val('');
                $('#ddlDepartment').html('<option value="">-- Select Department --</option>');
                $('#ddlDesignation').html('<option value="">-- Select Designation --</option>');

                // Clear the tracking variables
                currentCompanyId = null;
                currentDepartmentId = null;
                currentDesignationId = null;
                reloadDataTable();
            },
            change: function (event, ui) {
                if (!ui.item) {
                    $employeeInput.val('');
                    $employeeHidden.val('');
                    currentEmployeeId = null;
                    if (signatoryTable && isTableInitialized) {
                        signatoryTable.clear().draw();
                    }
                }
            }
        });
    }

    // Modal Functions
    function openCreateEditModal(id) {
        $.ajax({
            url: '/RequisitionSignatory/CreateOrEdit',
            type: 'GET',
            data: {companyId: companyId,id: id, integrateWith: currentIntegrateWith },
            success: function (html) {
                $('#modalCreateEdit').html(html);
                $('#modalCreateEditSignatory').modal('show');
                initializeCreateEditModal();
            },
            error: function () {
                showNotification('Error loading form', 'error');
            }
        });
    }

    function initializeCreateEditModal() {
        // Mode Toggle Buttons
        $(document).off('click', '#btnIndividualMode').on('click', '#btnIndividualMode', function () {
            $(this).addClass('active');
            $('#btnBulkMode').removeClass('active');
            $('#hdnAssignmentMode').val('individual');
            $('#individualModeSection').show();
            $('#bulkModeSection').hide();
        });

        $(document).off('click', '#btnBulkMode').on('click', '#btnBulkMode', function () {
            $(this).addClass('active');
            $('#btnIndividualMode').removeClass('active');
            $('#hdnAssignmentMode').val('bulk');
            $('#individualModeSection').hide();
            $('#bulkModeSection').show();
        });

        // Bulk Mode Cascading Dropdowns
        $(document).off('change', '#ddlBulkCompany').on('change', '#ddlBulkCompany', function () {
            var companyId = $(this).val();
            loadBulkDepartments(companyId);
            $('#employeePreview').hide();
        });

        $(document).off('change', '#ddlBulkDepartment').on('change', '#ddlBulkDepartment', function () {
            var departmentId = $(this).val();
            loadBulkDesignations(departmentId);
            $('#employeePreview').hide();
        });

        $(document).off('change', '#ddlBulkDesignation').on('change', '#ddlBulkDesignation', function () {
            $('#employeePreview').hide();
        });

        // Preview Employees Button
        $(document).off('click', '#btnPreviewEmployees').on('click', '#btnPreviewEmployees', function () {
            previewAffectedEmployees();
        });

        // Module Name dropdown change handler
        $(document).off('change', '#ddlModalIntegrateWith').on('change', '#ddlModalIntegrateWith', function () {
            var selectedModule = $(this).val();
            console.log('Module Name changed to:', selectedModule);
            $('#IntegrateWith').val(selectedModule);
        });

        // Destroy existing autocomplete if any
        if ($('#txtEmployeeMod').length && $('#txtEmployeeMod').hasClass('ui-autocomplete-input')) {
            $('#txtEmployeeMod').autocomplete('destroy');
        }
        if ($('#txtEmployee').length && $('#txtEmployee').hasClass('ui-autocomplete-input')) {
            $('#txtEmployee').autocomplete('destroy');
        }
        if ($('#txtSignatory').length && $('#txtSignatory').hasClass('ui-autocomplete-input')) {
            $('#txtSignatory').autocomplete('destroy');
        }
        if ($('#txtBulkSignatory').length && $('#txtBulkSignatory').hasClass('ui-autocomplete-input')) {
            $('#txtBulkSignatory').autocomplete('destroy');
        }

        // Employee Autocomplete (Individual Mode)
        if ($('#txtEmployee').length > 0) {
            console.log('Initializing txtEmployee autocomplete');
            $('#txtEmployee').autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: '/RequisitionSignatory/EmployeeAutoComplete',
                        type: 'POST',
                        dataType: 'json',
                        data: { prefix: request.term, companyId: $('#ddlModalCompany').val() },
                        success: function (data) {
                            response($.map(data, function (item) {
                                return {
                                    label: item.Name,
                                    value: item.Name,
                                    id: item.Id,
                                    designation: item.DesignationName
                                };
                            }));
                        },
                        error: function (xhr, status, error) {
                            console.error('txtEmployee autocomplete error:', error);
                            response([]);
                        }
                    });
                },
                minLength: 2,
                select: function (event, ui) {
                    console.log('txtEmployee selected:', ui.item);
                    $('#hdnEmployeeId').val(ui.item.id);

                    /*$('#txtDesignation').val(ui.item.designation);*/
                }
            });
        } else {
            console.error('txtEmployee element not found!');
        }


        if ($('#txtEmployeeMod').length > 0) {
            console.log('Initializing txtEmployee autocomplete');
            $('#txtEmployeeMod').autocomplete({
                source: function (request, response) {
                    console.log('txtEmployeeMod autocomplete source called with term:', request.term);
                    $.ajax({
                        url: '/RequisitionSignatory/EmployeeAutoComplete',
                        type: 'POST',
                        dataType: 'json',
                        data: { prefix: request.term, companyId: $('#ddlModalCompany').val() },
                        success: function (data) {
                            console.log('txtEmployee autocomplete data received:', data);
                            response($.map(data, function (item) {
                                return {
                                    label: item.Name,
                                    value: item.Name,
                                    id: item.Id,
                                    designation: item.DesignationName
                                };
                            }));
                        },
                        error: function (xhr, status, error) {
                            console.error('txtEmployee autocomplete error:', error);
                            response([]);
                        }
                    });
                },
                minLength: 2,
                select: function (event, ui) {
                    console.log('txtEmployee selected:', ui.item);
                    $('#hdnEmployeeIdMob').val(ui.item.id);
                    $('#txtDesignation').val(ui.item.designation);
                }
            });
        } else {
            console.error('txtEmployee element not found!');
        }

        // Signatory Autocomplete (Individual Mode)
        if ($('#txtSignatory').length > 0) {
            console.log('Initializing txtSignatory autocomplete');
            $('#txtSignatory').autocomplete({
                source: function (request, response) {
                    console.log('txtSignatory autocomplete source called with term:', request.term);
                    $.ajax({
                        url: '/RequisitionSignatory/EmployeeAutoComplete',
                        type: 'POST',
                        dataType: 'json',
                        data: { prefix: request.term, companyId: $('#ddlModalCompany').val() },
                        success: function (data) {
                            console.log('txtSignatory autocomplete data received:', data);
                            response($.map(data, function (item) {
                                return {
                                    label: item.Name,
                                    value: item.Name,
                                    id: item.Id,
                                    designation: item.DesignationName
                                };
                            }));
                        },
                        error: function (xhr, status, error) {
                            console.error('txtSignatory autocomplete error:', error);
                            response([]);
                        }
                    });
                },
                minLength: 2,
                select: function (event, ui) {
                    console.log('txtSignatory selected:', ui.item);
                    $('#hdnSignatoryEmpId').val(ui.item.id);
                    $('#txtDesignationSig').val(ui.item.designation);
                }
            });
        } else {
            console.error('txtSignatory element not found!');
        }

        // Bulk Signatory Autocomplete
        if ($('#txtBulkSignatory').length > 0) {
            console.log('Initializing txtBulkSignatory autocomplete');
            $('#txtBulkSignatory').autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: '/RequisitionSignatory/EmployeeAutoComplete',
                        type: 'POST',
                        dataType: 'json',
                        data: { prefix: request.term },
                        success: function (data) {
                            response($.map(data, function (item) {
                                return {
                                    label: item.Name,
                                    value: item.Name,
                                    id: item.Id
                                };
                            }));
                        },
                        error: function (xhr, status, error) {
                            console.error('txtBulkSignatory autocomplete error:', error);
                            response([]);
                        }
                    });
                },
                minLength: 2,
                select: function (event, ui) {
                    $('#hdnBulkSignatoryEmpId').val(ui.item.id);
                }
            });
        }

        // Form Submit
        $(document).off('submit', '#formCreateEdit').on('submit', '#formCreateEdit', function (e) {
            e.preventDefault();
            saveSignatory();
        });
    }

    // Bulk Assignment Helper Functions
    function loadBulkDepartments(companyId) {
        $('#ddlBulkDepartment').html('<option value="">-- All Departments --</option>');
        $('#ddlBulkDesignation').html('<option value="">-- All Designations --</option>');

        if (!companyId) return;

        $.ajax({
            url: '/RequisitionSignatory/GetDepartments',
            type: 'POST',
            data: { companyId: companyId },
            success: function (data) {
                $.each(data, function (i, item) {
                    $('#ddlBulkDepartment').append($('<option></option>').val(item.Value).html(item.Text));
                });
            }
        });
    }

    function loadBulkDesignations(departmentId) {
        $('#ddlBulkDesignation').html('<option value="">-- All Designations --</option>');

        if (!departmentId) return;

        $.ajax({
            url: '/RequisitionSignatory/GetDesignations',
            type: 'POST',
            data: { departmentId: departmentId },
            success: function (data) {
                $.each(data, function (i, item) {
                    $('#ddlBulkDesignation').append($('<option></option>').val(item.Value).html(item.Text));
                });
            }
        });
    }

    function previewAffectedEmployees() {
        var companyId = $('#ddlBulkCompany').val();
        var departmentId = $('#ddlBulkDepartment').val();
        var designationId = $('#ddlBulkDesignation').val();

        if (!companyId) {
            showNotification('Please select a company first', 'warning');
            return;
        }

        $.ajax({
            url: '/RequisitionSignatory/GetEmployeesByFilters',
            type: 'POST',
            data: {
                companyId: companyId,
                departmentId: departmentId || null,
                designationId: designationId || null
            },
            success: function (data) {
                if (!data || data.length === 0) {
                    showNotification('No employees found matching the selected filters', 'info');
                    $('#employeePreview').hide();
                    return;
                }

                $('#employeeCount').text(data.length);

                var listHtml = '<ul class="list-unstyled">';
                $.each(data, function (i, item) {
                    listHtml += '<li><i class="fa fa-user"></i> ' + item.Text + '</li>';
                });
                listHtml += '</ul>';

                $('#employeeList').html(listHtml);
                $('#employeePreview').show();
            },
            error: function (xhr, status, error) {
                console.error('Error loading employee preview:', error);
                console.error('Response:', xhr.responseText);
                showNotification('Error loading employee preview: ' + error, 'error');
            }
        });
    }

    function saveSignatory() {
        var form = $('#formCreateEdit');
        var formData = form.serialize();

        $.ajax({
            url: form.attr('action'),
            type: 'POST',
            data: formData,
            success: function (response) {
                if (response.success) {
                    showNotification(response.message, 'success');
                    $('#modalCreateEditSignatory').modal('hide');
                    window.location.reload();
                    //reloadDataTable();
                } else {
                    showNotification(response.message, 'error');
                }
            },
            error: function () {
                showNotification('Error saving signatory', 'error');
            }
        });
    }

    function openReplaceApproverModal() {
        $.ajax({
            url: '/RequisitionSignatory/ReplaceApprover',
            type: 'GET',
            data: { integrateWith: currentIntegrateWith, companyId: currentCompanyId },
            success: function (html) {
                $('#modalReplaceApprover').html(html);
                $('#modalReplaceApproverMain').modal('show');
                initializeReplaceApproverModal();
            },
            error: function () {
                showNotification('Error loading replace approver form', 'error');
            }
        });
    }

    function initializeReplaceApproverModal() {
        // From Employee Autocomplete
        $('#txtFromEmployee').autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '/RequisitionSignatory/EmployeeAutoComplete',
                    type: 'POST',
                    dataType: 'json',
                    data: { prefix: request.term },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return {
                                label: item.Name,
                                value: item.Name,
                                id: item.Id
                            };
                        }));
                    }
                });
            },
            minLength: 2,
            select: function (event, ui) {
                $('#hdnOldSignatoryEmpId').val(ui.item.id);
                loadEmployeeDetails(ui.item.id, 'from');
            }
        });

        // To Employee Autocomplete
        $('#txtToEmployee').autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '/RequisitionSignatory/EmployeeAutoComplete',
                    type: 'POST',
                    dataType: 'json',
                    data: { prefix: request.term },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return {
                                label: item.Name,
                                value: item.Name,
                                id: item.Id
                            };
                        }));
                    }
                });
            },
            minLength: 2,
            select: function (event, ui) {
                $('#hdnNewSignatoryEmpId').val(ui.item.id);
                loadEmployeeDetails(ui.item.id, 'to');
            }
        });

        // Form Submit
        $('#formReplaceApprover').on('submit', function (e) {
            e.preventDefault();
            replaceApprover();
        });
    }

    function loadEmployeeDetails(employeeId, type) {
        $.ajax({
            url: '/RequisitionSignatory/GetEmployeeDetails',
            type: 'POST',
            data: { employeeId: employeeId },
            success: function (response) {
                if (response.success) {
                    var prefix = type === 'from' ? 'From' : 'To';
                    $('#txt' + prefix + 'Company').val(response.data.company);
                    $('#txt' + prefix + 'Division').val(response.data.division);
                    $('#txt' + prefix + 'Department').val(response.data.department);
                    $('#txt' + prefix + 'Section').val(response.data.section);
                    $('#txt' + prefix + 'Designation').val(response.data.designation);
                    $('#txt' + prefix + 'AppointmentDate').val(response.data.appointmentDate);

                    var statusBadge = $('#div' + prefix + 'Status .status-badge');
                    statusBadge.text(response.data.status);
                    statusBadge.removeClass('status-active status-inactive');
                    statusBadge.addClass(response.data.status === 'Active' ? 'status-active' : 'status-inactive');
                }
            },
            error: function () {
                showNotification('Error loading employee details', 'error');
            }
        });
    }

    function replaceApprover() {
        var form = $('#formReplaceApprover');
        var formData = form.serialize();

        if (!confirm('Are you sure you want to replace this approver? This will update ALL records globally.')) {
            return;
        }

        $.ajax({
            url: form.attr('action'),
            type: 'POST',
            data: formData,
            success: function (response) {
                if (response.success) {
                    showNotification(response.message, 'success');
                    $('#modalReplaceApproverMain').modal('hide');
                    reloadDataTable();
                } else {
                    showNotification(response.message, 'error');
                }
            },
            error: function () {
                showNotification('Error replacing approver', 'error');
            }
        });
    }

    // Delete Functions
    window.confirmDelete = function (id) {
        deleteId = id;
        $('#modalDelete').modal('show');
    };

    function deleteSignatory() {
        debugger
        $.ajax({
            url: '/RequisitionSignatory/Delete',
            type: 'POST',
            data: {
                id: deleteId
            },
            success: function (response) {
                if (response.success) {
                    showNotification(response.message, 'success');
                    $('#modalDelete').modal('hide');
                    reloadDataTable();
                } else {
                    showNotification(response.message, 'error');
                }
            },
            error: function () {
                showNotification('Error deleting signatory', 'error');
            }
        });
    }

    // Edit Function (called from DataTable)
    window.editSignatory = function (id) {
        openCreateEditModal(id);
    };

    // Notification Function
    function showNotification(message, type) {
        // Using toastr if available, otherwise alert
        if (typeof toastr !== 'undefined') {
            toastr[type](message);
        } else {
            alert(message);
        }
    }

})();
