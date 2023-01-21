package com.fit.vut.Library.dtos;

import com.fit.vut.Library.entities.*;
import com.fit.vut.Library.enums.HardCopyBorrowingEnum;

import java.util.*;

public class HardCopyBorrowingDto extends BorrowingDto {

    private HardCopyBorrowingEnum state;

    private Date returnDate;

    private ExemplarLightDto exemplar;

    private List<FineDto> fines;

    public HardCopyBorrowingDto() {}

    public HardCopyBorrowingDto(Long id, Date dateOfBorrowStart, Date dateOfBorrowEnd, int borrowCounter, ReaderLightDto reader,
                                HardCopyBorrowingEnum state, Date returnDate, ExemplarLightDto exemplar,
                                List<FineDto> fines){
        super(id, dateOfBorrowStart, dateOfBorrowEnd, borrowCounter, reader);
        this.state = state;
        this.returnDate = returnDate;
        this.exemplar = exemplar;
        this.fines = fines;
    }


    public HardCopyBorrowingEnum getState() { return state; }

    public void setState(HardCopyBorrowingEnum state) { this.state = state; }

    public Date getReturnDate() { return returnDate; }

    public void setReturnDate(Date returnDate) { this.returnDate = returnDate; }

    public ExemplarLightDto getExemplar() {
        return exemplar;
    }

    public void setExemplar(ExemplarLightDto copy) {
        this.exemplar = copy;
    }

    public List<FineDto> getFines() {
        return fines;
    }

    public void setFines(List<FineDto> fines) {
        this.fines = fines;
    }

    public HardCopyBorrowing toEntity() {
        List<Fine> fineEntities = getFines() == null ? new ArrayList() : getFines().stream().map(x -> x.toEntity()).toList();
        return new HardCopyBorrowing(id, dateOfBorrowStart, dateOfBorrowEnd, borrowCounter, reader.toEntity(),
                state, returnDate, fineEntities, exemplar.toHardCopyExemplarEntity());
    }
}
