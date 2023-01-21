package com.fit.vut.Library.entities;

import javax.persistence.*;
import java.util.Date;

@MappedSuperclass
public abstract class Borrowing extends EntityBase{

    @Column(name = "dateOfBorrowStart", nullable = false)
    protected Date dateOfBorrowStart;

    @Column(name = "dateOfBorrowEnd", nullable = false)
    protected Date dateOfBorrowEnd;

    @Column(name = "extensionCounter", nullable = false)
    protected int extensionCounter;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name="readerId", nullable = false)
    protected Reader reader;

    public Borrowing(){}

    public Borrowing(Long id, Date dateOfBorrowStart, Date dateOfBorrowEnd, int extensionCounter, Reader reader) {
        super(id);
        this.dateOfBorrowStart = dateOfBorrowStart;
        this.dateOfBorrowEnd = dateOfBorrowEnd;
        this.extensionCounter = extensionCounter;
        this.reader = reader;
    }

    public Borrowing(Date dateOfBorrowStart, Date dateOfBorrowEnd, int extensionCounter, Reader reader) {
        this.dateOfBorrowStart = dateOfBorrowStart;
        this.dateOfBorrowEnd = dateOfBorrowEnd;
        this.extensionCounter = extensionCounter;
        this.reader = reader;
    }

    public Reader getReader() {
        return reader;
    }

    public void setReader(Reader reader) {
        this.reader = reader;
    }

    public Date getDateOfBorrowStart() {
        return dateOfBorrowStart;
    }

    public void setDateOfBorrowStart(Date dateOfBorrowStart) {
        this.dateOfBorrowStart = dateOfBorrowStart;
    }

    public Date getDateOfBorrowEnd() {
        return dateOfBorrowEnd;
    }

    public void setDateOfBorrowEnd(Date dateOfBorrowEnd) {
        this.dateOfBorrowEnd = dateOfBorrowEnd;
    }

    public int getExtensionCounter() {
        return extensionCounter;
    }

    public void setExtensionCounter(int extensionCounter) {
        this.extensionCounter = extensionCounter;
    }
}
