package com.fit.vut.Library.controllers;

import com.fit.vut.Library.dtos.FieldDto;
import com.fit.vut.Library.services.FieldService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.server.ResponseStatusException;

import java.util.List;

@RestController
@RequestMapping("/fields")
@Tag(name="Fields")
@ApiResponses(value = {
    @ApiResponse(responseCode = "200", description = "Successful operation"),
    @ApiResponse(responseCode = "400", description = "Bad request"),
    @ApiResponse(responseCode = "500", description = "Internal server error")
})
public class FieldsController {

    private final FieldService service;

    public FieldsController(FieldService service) {
        this.service = service;
    }

    @Operation(summary = "Gets field with given id.")
    @GetMapping(value = "/{id}", produces = {"application/json"})
    @ApiResponse(responseCode = "404", description = "Field not found")
    public FieldDto getById(@PathVariable Long id) {
        FieldDto field = service.getById(id);
        if (field == null)
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Field with id " + id + " not found.");

        return field;
    }

    @Operation(summary = "Gets all fields optionally filtered by name.")
    @GetMapping(value = "/", produces = {"application/json"})
    public List<FieldDto> get(@RequestParam(required = false) String name) {
        return service.get(name);
    }

    @Operation(summary = "Creates new field.")
    @PostMapping()
    public void create(@RequestBody FieldDto field) {
        if (field.getId() != 0)
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Id must be 0 when creating new field.");

        service.save(field);
    }

    @Operation(summary = "Updates field.")
    @PutMapping()
    public void update(@RequestBody FieldDto field) {
        if (field.getId() <= 0)
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Id must be greater than 0 when updating a field.");

        service.save(field);
    }

    @Operation(summary = "Deletes a field.")
    @DeleteMapping("/{id}")
    public void delete(@PathVariable Long id) { service.delete(id); }
}
