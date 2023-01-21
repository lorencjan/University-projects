package com.fit.vut.Library.controllers;

import com.fit.vut.Library.dtos.GenreDto;
import com.fit.vut.Library.services.GenreService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.server.ResponseStatusException;

import java.util.List;

@RestController
@RequestMapping("/genres")
@Tag(name="Genres")
@ApiResponses(value = {
    @ApiResponse(responseCode = "200", description = "Successful operation"),
    @ApiResponse(responseCode = "400", description = "Bad request"),
    @ApiResponse(responseCode = "500", description = "Internal server error")
})
public class GenresController {

    private final GenreService service;

    public GenresController(GenreService service) {
        this.service = service;
    }

    @Operation(summary = "Gets genre with given id.")
    @GetMapping(value = "/{id}", produces = {"application/json"})
    @ApiResponse(responseCode = "404", description = "Genre not found")
    public GenreDto getById(@PathVariable Long id) {
        GenreDto genre = service.getById(id);
        if (genre == null)
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Genre with id " + id + " not found.");

        return genre;
    }

    @Operation(summary = "Gets all genres optionally filtered by name.")
    @GetMapping(value = "/", produces = {"application/json"})
    public List<GenreDto> get(@RequestParam(required = false) String name) {
        return service.get(name);
    }

    @Operation(summary = "Creates new genre.")
    @PostMapping()
    public void create(@RequestBody GenreDto genre) {
        if (genre.getId() != 0)
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Id must be 0 when creating new genre.");

        service.save(genre);
    }

    @Operation(summary = "Updates genre.")
    @PutMapping()
    public void update(@RequestBody GenreDto genre) {
        if (genre.getId() <= 0)
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Id must be greater than 0 when updating a genre.");

        service.save(genre);
    }

    @Operation(summary = "Deletes a genre.")
    @DeleteMapping("/{id}")
    public void delete(@PathVariable Long id) { service.delete(id); }
}
