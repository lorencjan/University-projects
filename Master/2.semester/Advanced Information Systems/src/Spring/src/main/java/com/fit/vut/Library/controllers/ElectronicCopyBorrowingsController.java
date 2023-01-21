package com.fit.vut.Library.controllers;

import com.fit.vut.Library.dtos.ElectronicCopyBorrowingDto;
import com.fit.vut.Library.services.ElectronicCopyBorrowingService;
import com.fit.vut.Library.services.filters.BorrowingFilter;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.server.ResponseStatusException;

import java.util.List;

@RestController
@RequestMapping("/electronic-copy-borrowings")
@Tag(name="Electronic copy borrowings")
@ApiResponses(value = {
    @ApiResponse(responseCode = "200", description = "Successful operation"),
    @ApiResponse(responseCode = "400", description = "Bad request"),
    @ApiResponse(responseCode = "500", description = "Internal server error")
})
public class ElectronicCopyBorrowingsController {

    private final ElectronicCopyBorrowingService service;

    public ElectronicCopyBorrowingsController(ElectronicCopyBorrowingService service) {
        this.service = service;
    }

    @Operation(summary = "Gets electronic copy borrowing with given id.")
    @GetMapping(value = "/{id}", produces = {"application/json"})
    @ApiResponse(responseCode = "404", description = "Borrowing not found")
    public ElectronicCopyBorrowingDto getById(@PathVariable Long id) {
        ElectronicCopyBorrowingDto borrowing = service.getById(id);
        if (borrowing == null)
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Electronic copy borrowing with id " + id + " not found.");

        return borrowing;
    }

    @Operation(summary = "Gets all electronic copy borrowings meeting filter requirements.")
    @GetMapping(value = "/", produces = {"application/json"})
    public List<ElectronicCopyBorrowingDto> get(BorrowingFilter filter) {
        return service.get(filter);
    }

    @Operation(summary = "Creates new electronic copy borrowing.")
    @PostMapping()
    public void create(@RequestBody ElectronicCopyBorrowingDto borrowing) {
        if (borrowing.getId() != 0)
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Id must be 0 when creating new borrowing.");

        service.save(borrowing);
    }

    @Operation(summary = "Updates electronic copy borrowing.")
    @PutMapping()
    public void update(@RequestBody ElectronicCopyBorrowingDto borrowing) {
        if (borrowing.getId() <= 0)
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Id must be greater than 0 when updating a borrowing.");

        service.save(borrowing);
    }

    @Operation(summary = "Deletes an electronic copy borrowing.")
    @DeleteMapping("/{id}")
    public void delete(@PathVariable Long id) { service.delete(id); }
}
