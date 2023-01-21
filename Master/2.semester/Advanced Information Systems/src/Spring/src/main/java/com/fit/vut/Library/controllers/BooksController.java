package com.fit.vut.Library.controllers;

import com.fit.vut.Library.dtos.BookDto;
import com.fit.vut.Library.services.BookService;
import com.fit.vut.Library.services.filters.BookFilter;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.server.ResponseStatusException;

import java.util.List;

@RestController
@RequestMapping("/books")
@Tag(name="Books")
@ApiResponses(value = {
    @ApiResponse(responseCode = "200", description = "Successful operation"),
    @ApiResponse(responseCode = "400", description = "Bad request"),
    @ApiResponse(responseCode = "500", description = "Internal server error")
})
public class BooksController {

    private final BookService service;

    public BooksController(BookService service) {
        this.service = service;
    }

    @Operation(summary = "Gets book with given id.")
    @GetMapping(value = "/{id}", produces = {"application/json"})
    @ApiResponse(responseCode = "404", description = "Book not found")
    public BookDto getById(@PathVariable Long id) {
        BookDto book = service.getById(id);
        if (book == null)
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Book with id " + id + " not found.");

        return book;
    }

    @Operation(summary = "Gets all books meeting filter requirements.")
    @GetMapping(value = "", produces = {"application/json"})
    public List<BookDto> get(BookFilter filter) {
        return service.get(filter);
    }

    @Operation(summary = "Creates new book.")
    @PostMapping()
    public void create(@RequestBody BookDto book) {
        if (book.getId() != 0)
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Id must be 0 when creating new book.");

        service.save(book);
    }

    @Operation(summary = "Updates book.")
    @PutMapping()
    public void update(@RequestBody BookDto book) {
        if (book.getId() <= 0)
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Id must be greater than 0 when updating a book.");

        service.save(book);
    }

    @Operation(summary = "Deletes a book.")
    @DeleteMapping("/{id}")
    public void delete(@PathVariable Long id) { service.delete(id); }
}
