package com.fit.vut.Library.dtos;

import java.util.Date;

public abstract class BorrowingDto extends DtoBase{

    protected Date dateOfBorrowStart;

    protected Date dateOfBorrowEnd;

    protected int borrowCounter;

    protected ReaderLightDto reader;

    public BorrowingDto() {}

    public BorrowingDto(Long id, Date dateOfBorrowStart, Date dateOfBorrowEnd, int borrowCounter, ReaderLightDto reader){
        super(id);
        this.dateOfBorrowStart = dateOfBorrowStart;
        this.dateOfBorrowEnd = dateOfBorrowEnd;
        this.borrowCounter = borrowCounter;
        this.reader = reader;
    }

    public Date getDateOfBorrowStart() { return dateOfBorrowStart; }

    public void setDateOfBorrowStart(Date dateOfBorrowStart) { this.dateOfBorrowStart = dateOfBorrowStart; }

    public Date getDateOfBorrowEnd() { return dateOfBorrowEnd; }

    public void setDateOfBorrowEnd(Date dateOfBorrowEnd) { this.dateOfBorrowEnd = dateOfBorrowEnd; }

    public int getBorrowCounter() { return borrowCounter; }

    public void setBorrowCounter(int borrowCounter) { this.borrowCounter = borrowCounter; }

    public ReaderLightDto getReader() { return reader; }

    public void setReader(ReaderLightDto reader) { this.reader = reader; }
}
