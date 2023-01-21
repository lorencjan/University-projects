package com.fit.vut.Library.controllers;

import com.fit.vut.Library.dtos.AuthorDto;
import com.fit.vut.Library.services.AuthorService;
import com.fit.vut.Library.services.filters.AuthorFilter;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.server.ResponseStatusException;
import java.util.List;

@RestController
@RequestMapping("/authors")
@Tag(name="Authors")
@ApiResponses(value = {
    @ApiResponse(responseCode = "200", description = "Successful operation"),
    @ApiResponse(responseCode = "400", description = "Bad request"),
    @ApiResponse(responseCode = "500", description = "Internal server error")
})
public class AuthorsController {

    private final AuthorService service;

    public AuthorsController(AuthorService service) {
        this.service = service;
    }

    @Operation(summary = "Gets author with given id.")
    @GetMapping(value = "/{id}", produces = {"application/json"})
    @ApiResponse(responseCode = "404", description = "Author not found")
    public AuthorDto getById(@PathVariable Long id) {
        AuthorDto author = service.getById(id);
        if (author == null)
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Author with id " + id + " not found.");

        return author;
    }

    @Operation(summary = "Gets all authors meeting given filter.")
    @GetMapping(value = "/", produces = {"application/json"})
    public List<AuthorDto> getByFilter(AuthorFilter filter) {
        return service.get(filter);
    }

    @Operation(summary = "Creates new author.")
    @PostMapping()
    public void create(@RequestBody AuthorDto author) {
        if (author.getId() != 0)
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Id must be 0 when creating new author.");

        service.save(author);
    }

    @Operation(summary = "Updates an author.")
    @PutMapping()
    public void update(@RequestBody AuthorDto author) {
        if (author.getId() <= 0)
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Id must be greater than 0 when updating an author.");

        service.save(author);
    }

    @Operation(summary = "Deletes an author.")
    @DeleteMapping("/{id}")
    public void delete(@PathVariable Long id) {
        service.delete(id);
    }
}
