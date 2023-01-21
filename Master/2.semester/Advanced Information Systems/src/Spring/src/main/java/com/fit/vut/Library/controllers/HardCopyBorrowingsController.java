package com.fit.vut.Library.controllers;

import com.fit.vut.Library.dtos.HardCopyBorrowingDto;
import com.fit.vut.Library.services.HardCopyBorrowingService;
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
@RequestMapping("/hard-copy-borrowings")
@Tag(name="Hard copy borrowings")
@ApiResponses(value = {
    @ApiResponse(responseCode = "200", description = "Successful operation"),
    @ApiResponse(responseCode = "400", description = "Bad request"),
    @ApiResponse(responseCode = "500", description = "Internal server error")
})
public class HardCopyBorrowingsController {

    private final HardCopyBorrowingService service;

    public HardCopyBorrowingsController(HardCopyBorrowingService service) {
        this.service = service;
    }

    @Operation(summary = "Gets hard copy borrowing with given id.")
    @GetMapping(value = "/{id}", produces = {"application/json"})
    @ApiResponse(responseCode = "404", description = "Borrowing not found")
    public HardCopyBorrowingDto getById(@PathVariable Long id) {
        HardCopyBorrowingDto borrowing = service.getById(id);
        if (borrowing == null)
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Hard copy borrowing with id " + id + " not found.");

        return borrowing;
    }

    @Operation(summary = "Gets all hard copy borrowings meeting filter requirements.")
    @GetMapping(value = "/", produces = {"application/json"})
    public List<HardCopyBorrowingDto> get(BorrowingFilter filter) {
        return service.get(filter);
    }

    @Operation(summary = "Creates new hard copy borrowing.")
    @PostMapping()
    public void create(@RequestBody HardCopyBorrowingDto borrowing) {
        if (borrowing.getId() != 0)
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Id must be 0 when creating new borrowing.");

        service.save(borrowing);
    }

    @Operation(summary = "Updates hard copy borrowing.")
    @PutMapping()
    public void update(@RequestBody HardCopyBorrowingDto borrowing) {
        if (borrowing.getId() <= 0)
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Id must be greater than 0 when updating a borrowing.");

        Boolean prolonged = service.prolong(borrowing);
        if (! prolonged){
            throw  new ResponseStatusException(HttpStatus.BAD_REQUEST, "Can not prolong borrowing.");
        }
    }

    @Operation(summary = "Deletes a hard copy borrowing.")
    @DeleteMapping("/{id}")
    public void delete(@PathVariable Long id) { service.delete(id); }
}
