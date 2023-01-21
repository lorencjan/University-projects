package com.fit.vut.Library.application;

import com.fit.vut.Library.dtos.*;
import com.fit.vut.Library.enums.FineEnum;
import com.fit.vut.Library.services.*;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Component;

import java.util.List;

@Component
public class NoticeScheduler
{
    private final int finePeriod = 7;
    private final int newFineCost = 250;
    private final int additionalFineCost = 500;
    private final String fineEmailHeader = "PIS-Library - fine";
    private final String borrowingEndsEmailHeader = "PIS-Library - borrowing ends soon";

    private HardCopyBorrowingService borrowingService;
    private FineService fineService;
    private ReaderService readerService;

    public NoticeScheduler(HardCopyBorrowingService borrowingService, FineService fineService, ReaderService readerService){
        this.borrowingService = borrowingService;
        this.fineService = fineService;
        this.readerService = readerService;
    }

    @Scheduled(cron = "0 0 0 * * *")
    public void sendNotices() {
        sendBorrowingEndsSoonNotices();
        sendNewFineNotices();
        sendAdditionalFineNotices();
    }

    private void sendNewFineNotices(){
        List<HardCopyBorrowingDto> borrowings = borrowingService.getBorrowingsThatExpireIn(0);
        for (HardCopyBorrowingDto b : borrowings) {
            fineService.save(new FineDto(0L, newFineCost, FineEnum.UNPAID, b.getId()));
            ReaderDto reader = readerService.getById(b.getReader().getId());

            String msg =
                "Dear " + reader.getFullname() + ",\n\n" +
                "you haven't returned borrowed title " + b.getExemplar().getTitleName() + " in time. " +
                "Therefore you'll be charged " + newFineCost + " Kč.\n" +
                "You have " + finePeriod + " days to return the title and pay the fine, otherwise another fine will be charged.\n\n" +
                "(Do not answer this e-mail as it's automatically generated.)";

            EmailManager.sendMail(reader.getEmail(), fineEmailHeader, msg);
        }
    }

    private void sendAdditionalFineNotices(){
        List<HardCopyBorrowingDto> borrowings = borrowingService.getBorrowingsWithExpiredFine(finePeriod);
        for (HardCopyBorrowingDto b : borrowings) {
            fineService.save(new FineDto(0L, additionalFineCost, FineEnum.UNPAID, b.getId()));
            ReaderDto reader = readerService.getById(b.getReader().getId());

            String msg =
                "Dear " + reader.getFullname() + ",\n\n" +
                "haven't paid fine for not returning title " + b.getExemplar().getTitleName() + ". " +
                "Therefore additional fine amounting " + additionalFineCost + " Kč is charged.\n" +
                "You have " + finePeriod + " days to return the title and pay the fine, otherwise yet another fine will be charged.\n\n" +
                "(Do not answer this e-mail as it's automatically generated.)";

            EmailManager.sendMail(reader.getEmail(), fineEmailHeader, msg);
        }
    }

    private void sendBorrowingEndsSoonNotices(){
        List<HardCopyBorrowingDto> borrowings = borrowingService.getBorrowingsThatExpireIn(finePeriod);
        for (HardCopyBorrowingDto b : borrowings) {
            ReaderDto reader = readerService.getById(b.getReader().getId());

            String msg =
                "Dear " + reader.getFullname() + ",\n\n" +
                "we'd like to remind you that your borrowing of title " + b.getExemplar().getTitleName() + " ends in " + finePeriod + " days.\n" +
                "You have until then to return the title, otherwise a fine will be charged.\n\n" +
                "(Do not answer this e-mail as it's automatically generated.)";

            EmailManager.sendMail(reader.getEmail(), borrowingEndsEmailHeader, msg);
        }
    }
}
