package com.fit.vut.Library.controllers;

import com.fit.vut.Library.dtos.*;
import com.fit.vut.Library.services.ReaderService;
import com.fit.vut.Library.services.filters.UserFilter;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.server.ResponseStatusException;

import java.util.List;

@RestController
@RequestMapping("/readers")
@Tag(name="Readers")
@ApiResponses(value = {
    @ApiResponse(responseCode = "200", description = "Successful operation"),
    @ApiResponse(responseCode = "400", description = "Bad request"),
    @ApiResponse(responseCode = "500", description = "Internal server error")
})
public class ReadersController {

    private final ReaderService service;

    public ReadersController(ReaderService service) {
        this.service = service;
    }

    @Operation(summary = "Gets reader with given id.")
    @GetMapping(value = "/{id}", produces = {"application/json"})
    @ApiResponse(responseCode = "404", description = "Reader not found")
    public ReaderDto getById(@PathVariable Long id) {
        ReaderDto reader = service.getById(id);
        if (reader == null)
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Reader with id " + id + " not found.");

        return reader;
    }

    @Operation(summary = "Gets all readers meeting filter requirements.")
    @GetMapping(value = "/", produces = {"application/json"})
    public List<ReaderDto> get(UserFilter filter) {
        return service.get(filter);
    }

    @Operation(summary = "Creates new reader.")
    @PostMapping()
    public void create(@RequestBody FullUserDto reader) {
        if (reader.getId() != 0)
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Id must be 0 when creating new reader.");
        try {
            service.create(reader);
        }
        catch (Exception e){
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "User already exists.");
        }
    }

    @Operation(summary = "Updates reader.")
    @PutMapping()
    public void update(@RequestBody ReaderDto reader) {
        if (reader.getId() <= 0)
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Id must be greater than 0 when updating a reader.");

        service.update(reader);
    }

    @Operation(summary = "Updates reader's password.")
    @PutMapping(value = "/update-password")
    @ApiResponse(responseCode = "404", description = "Reader not found")
    public void updatePassword(@RequestBody NewPasswordDto newPassword) {
        if (service.getById(newPassword.getUserId()) == null)
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Reader with " + newPassword.getUserId() + " id doesn't exist.");

        if (! service.updatePassword(newPassword) ){
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Old password doesnt match.");
        }
    }

    @Operation(summary = "Updates reader's password.")
    @PutMapping(value = "/change-password")
    @ApiResponse(responseCode = "404", description = "Reader not found")
    public void changePassword(@RequestBody ChangePasswordDto changePasswordDto) {
        if (service.getByEmail(changePasswordDto.getUserEmail()) == null)
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Reader with " + changePasswordDto.getUserEmail() + " email doesn't exist.");

        service.changePassword(changePasswordDto);

    }
    
    @Operation(summary = "Deletes a reader.")
    @DeleteMapping("/{id}")
    public void delete(@PathVariable Long id) { service.delete(id); }

    @Operation(summary = "Verifies reader's credentials and returns reader object if valid.")
    @PostMapping(value = "/authenticate", produces = {"application/json"})
    @ApiResponse(responseCode = "404", description = "Reader not found")
    public ReaderDto authenticate(@RequestBody CredentialsDto credentials) {
        ReaderDto reader = service.getByCredentials(credentials);
        if (reader == null)
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Reader with given credentials not found.");

        return reader;
    }
}
