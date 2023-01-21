package com.fit.vut.Library.controllers;

import com.fit.vut.Library.dtos.ElectronicCopyExemplarDto;
import com.fit.vut.Library.services.ElectronicCopyExemplarService;
import com.fit.vut.Library.services.filters.ExemplarFilter;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.server.ResponseStatusException;

import java.util.List;

@RestController
@RequestMapping("/electronic-copy-exemplars")
@Tag(name="Electronic copy exemplars")
@ApiResponses(value = {
    @ApiResponse(responseCode = "200", description = "Successful operation"),
    @ApiResponse(responseCode = "400", description = "Bad request"),
    @ApiResponse(responseCode = "500", description = "Internal server error")
})
public class ElectronicCopyExemplarsController {

    private final ElectronicCopyExemplarService service;

    public ElectronicCopyExemplarsController(ElectronicCopyExemplarService service) {
        this.service = service;
    }

    @Operation(summary = "Gets electronic copy exemplar with given id.")
    @GetMapping(value = "/{id}", produces = {"application/json"})
    @ApiResponse(responseCode = "404", description = "Exemplar not found")
    public ElectronicCopyExemplarDto getById(@PathVariable Long id) {
        ElectronicCopyExemplarDto exemplar = service.getById(id);
        if (exemplar == null)
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Electronic copy exemplar with id " + id + " not found.");

        return exemplar;
    }

    @Operation(summary = "Gets all electronic copy exemplars meeting filter requirements.")
    @GetMapping(value = "/", produces = {"application/json"})
    public List<ElectronicCopyExemplarDto> get(ExemplarFilter filter) {
        return service.get(filter);
    }

    @Operation(summary = "Creates new electronic copy exemplar.")
    @PostMapping()
    public void create(@RequestBody ElectronicCopyExemplarDto exemplar) {
        if (exemplar.getId() != 0)
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Id must be 0 when creating new exemplar.");

        service.save(exemplar);
    }

    @Operation(summary = "Updates electronic copy exemplar.")
    @PutMapping()
    public void update(@RequestBody ElectronicCopyExemplarDto exemplar) {
        if (exemplar.getId() <= 0)
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Id must be greater than 0 when updating an exemplar.");

        service.save(exemplar);
    }

    @Operation(summary = "Deletes a electronic copy exemplar.")
    @DeleteMapping("/{id}")
    public void delete(@PathVariable Long id) { service.delete(id); }
}
