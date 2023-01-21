package com.fit.vut.Library.controllers;

import com.fit.vut.Library.dtos.FineDto;
import com.fit.vut.Library.services.FineService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.server.ResponseStatusException;

import java.util.List;
import java.util.Optional;

@RestController
@RequestMapping("/fines")
@Tag(name="Fines")
@ApiResponses(value = {
    @ApiResponse(responseCode = "200", description = "Successful operation"),
    @ApiResponse(responseCode = "400", description = "Bad request"),
    @ApiResponse(responseCode = "500", description = "Internal server error")
})
public class FinesController {

    private final FineService service;

    public FinesController(FineService service) {
        this.service = service;
    }

    @Operation(summary = "Gets fine with given id.")
    @GetMapping(value = "/{id}", produces = {"application/json"})
    @ApiResponse(responseCode = "404", description = "Fine not found")
    public FineDto getById(@PathVariable Long id) {
        FineDto fine = service.getById(id);
        if (fine == null)
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Fine with id " + id + " not found.");

        return fine;
    }

    @Operation(summary = "Gets all fines optionally filtered by state.")
    @GetMapping(value = "/", produces = {"application/json"})
    public List<FineDto> get(@RequestParam(required = false) Optional<Integer> state) {
        return service.get(state);
    }

    @Operation(summary = "Creates new fine.")
    @PostMapping()
    public void create(@RequestBody FineDto fine) {
        if (fine.getId() != 0)
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Id must be 0 when creating new fine.");

        service.save(fine);
    }

    @Operation(summary = "Updates fine.")
    @PutMapping()
    public void update(@RequestBody FineDto fine) {
        if (fine.getId() <= 0)
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Id must be greater than 0 when updating a fine.");

        service.save(fine);
    }

    @Operation(summary = "Deletes a fine.")
    @DeleteMapping("/{id}")
    public void delete(@PathVariable Long id) { service.delete(id); }
}
