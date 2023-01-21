package com.fit.vut.Library.controllers;

import com.fit.vut.Library.dtos.HardCopyExemplarDto;
import com.fit.vut.Library.services.HardCopyExemplarService;
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
@RequestMapping("/hard-copy-exemplars")
@Tag(name="Hard copy exemplars")
@ApiResponses(value = {
    @ApiResponse(responseCode = "200", description = "Successful operation"),
    @ApiResponse(responseCode = "400", description = "Bad request"),
    @ApiResponse(responseCode = "500", description = "Internal server error")
})
public class HardCopyExemplarsController {

    private final HardCopyExemplarService service;

    public HardCopyExemplarsController(HardCopyExemplarService service) {
        this.service = service;
    }

    @Operation(summary = "Gets hard copy exemplar with given id.")
    @GetMapping(value = "/{id}", produces = {"application/json"})
    @ApiResponse(responseCode = "404", description = "Exemplar not found")
    public HardCopyExemplarDto getById(@PathVariable Long id) {
        HardCopyExemplarDto exemplar = service.getById(id);
        if (exemplar == null)
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Hard copy exemplar with id " + id + " not found.");

        return exemplar;
    }

    @Operation(summary = "Gets all hard copy exemplars meeting filter requirements.")
    @GetMapping(value = "/", produces = {"application/json"})
    public List<HardCopyExemplarDto> get(ExemplarFilter filter) {
        return service.get(filter);
    }

    @Operation(summary = "Creates new hard copy exemplar.")
    @PostMapping()
    public void create(@RequestBody HardCopyExemplarDto exemplar) {
        if (exemplar.getId() != 0)
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Id must be 0 when creating new exemplar.");

        service.save(exemplar);
    }

    @Operation(summary = "Updates hard copy exemplar.")
    @PutMapping()
    public void update(@RequestBody HardCopyExemplarDto exemplar) {
        if (exemplar.getId() <= 0)
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Id must be greater than 0 when updating an exemplar.");

        service.save(exemplar);
    }

    @Operation(summary = "Deletes a hard copy exemplar.")
    @DeleteMapping("/{id}")
    public void delete(@PathVariable Long id) { service.delete(id); }
}
