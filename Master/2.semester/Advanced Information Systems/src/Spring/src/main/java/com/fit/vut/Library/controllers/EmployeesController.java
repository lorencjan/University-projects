package com.fit.vut.Library.controllers;

import com.fit.vut.Library.dtos.CredentialsDto;
import com.fit.vut.Library.dtos.EmployeeDto;
import com.fit.vut.Library.dtos.FullUserDto;
import com.fit.vut.Library.dtos.NewPasswordDto;
import com.fit.vut.Library.services.EmployeeService;
import com.fit.vut.Library.services.filters.UserFilter;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.server.ResponseStatusException;

import java.util.List;

@RestController
@RequestMapping("/employees")
@Tag(name="Employees")
@ApiResponses(value = {
    @ApiResponse(responseCode = "200", description = "Successful operation"),
    @ApiResponse(responseCode = "400", description = "Bad request"),
    @ApiResponse(responseCode = "500", description = "Internal server error")
})
public class EmployeesController {

    private final EmployeeService service;

    public EmployeesController(EmployeeService service) {
        this.service = service;
    }

    @Operation(summary = "Gets employee with given id.")
    @GetMapping(value = "/{id}", produces = {"application/json"})
    @ApiResponse(responseCode = "404", description = "Employee not found")
    public EmployeeDto getById(@PathVariable Long id) {
        EmployeeDto employee = service.getById(id);
        if (employee == null)
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Employee with id " + id + " not found.");

        return employee;
    }

    @Operation(summary = "Gets all employees meeting filter requirements.")
    @GetMapping(value = "/", produces = {"application/json"})
    public List<EmployeeDto> get(UserFilter filter) {
        return service.get(filter);
    }

    @Operation(summary = "Creates new employee.")
    @PostMapping()
    public void create(@RequestBody FullUserDto employee) {
        if (employee.getId() != 0)
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Id must be 0 when creating new employee.");

        try{
            service.create(employee);
        }
        catch (Exception e){
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Employee with given email already exists");
        }

    }

    @Operation(summary = "Updates employee.")
    @PutMapping()
    public void update(@RequestBody EmployeeDto employee) {
        if (employee.getId() <= 0)
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Id must be greater than 0 when updating an employee.");

        service.update(employee);
    }

    @Operation(summary = "Updates employee's password.")
    @PutMapping(value = "/update-password")
    @ApiResponse(responseCode = "404", description = "Employee not found")
    public void updatePassword(@RequestBody NewPasswordDto newPassword) {
        if (service.getById(newPassword.getUserId()) == null)
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Employee with " + newPassword.getUserId() + " id doesn't exist.");

        service.updatePassword(newPassword);
    }

    @Operation(summary = "Deletes an employee.")
    @DeleteMapping("/{id}")
    public void delete(@PathVariable Long id) { service.delete(id); }

    @Operation(summary = "Verifies employee's credentials and returns employee object if valid.")
    @PostMapping(value = "/authenticate", produces = {"application/json"})
    @ApiResponse(responseCode = "404", description = "Employee not found")
    public EmployeeDto authenticate(@RequestBody CredentialsDto credentials) {
        EmployeeDto employee = service.getByCredentials(credentials);
        if (employee == null)
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Employee with given credentials not found.");

        return employee;
    }
}
