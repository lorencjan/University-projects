package com.fit.vut.Library.controllers;

import com.fit.vut.Library.dtos.MagazineDto;
import com.fit.vut.Library.services.MagazineService;
import com.fit.vut.Library.services.filters.MagazineFilter;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.server.ResponseStatusException;

import java.util.List;

@RestController
@RequestMapping("/magazines")
@Tag(name="Magazines")
@ApiResponses(value = {
    @ApiResponse(responseCode = "200", description = "Successful operation"),
    @ApiResponse(responseCode = "400", description = "Bad request"),
    @ApiResponse(responseCode = "500", description = "Internal server error")
})
public class MagazinesController {

    private final MagazineService service;

    public MagazinesController(MagazineService service) {
        this.service = service;
    }

    @Operation(summary = "Gets magazine with given id.")
    @GetMapping(value = "/{id}", produces = {"application/json"})
    @ApiResponse(responseCode = "404", description = "Magazine not found")
    public MagazineDto getById(@PathVariable Long id) {
        MagazineDto magazine = service.getById(id);
        if (magazine == null)
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Magazine with id " + id + " not found.");

        return magazine;
    }

    @Operation(summary = "Gets all magazines meeting filter requirements.")
    @GetMapping(value = "/", produces = {"application/json"})
    public List<MagazineDto> get(MagazineFilter filter) {
        return service.get(filter);
    }

    @Operation(summary = "Creates new magazine.")
    @PostMapping()
    public void create(@RequestBody MagazineDto magazine) {
        if (magazine.getId() != 0)
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Id must be 0 when creating new magazine.");

        service.save(magazine);
    }

    @Operation(summary = "Updates magazine.")
    @PutMapping()
    public void update(@RequestBody MagazineDto magazine) {
        if (magazine.getId() <= 0)
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Id must be greater than 0 when updating a magazine.");

        service.save(magazine);
    }

    @Operation(summary = "Deletes a magazine.")
    @DeleteMapping("/{id}")
    public void delete(@PathVariable Long id) { service.delete(id); }
}
