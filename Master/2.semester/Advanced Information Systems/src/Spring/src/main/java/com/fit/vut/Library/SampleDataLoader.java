package com.fit.vut.Library;

import com.fit.vut.Library.application.PasswordEncryptor;
import com.fit.vut.Library.entities.*;
import com.fit.vut.Library.enums.*;
import com.fit.vut.Library.repositories.*;
import com.github.javafaker.Faker;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.CommandLineRunner;
import org.springframework.stereotype.Component;
import org.springframework.transaction.annotation.Transactional;

import java.util.*;
import java.util.concurrent.TimeUnit;
import java.util.stream.*;

@Component
@Transactional
public class SampleDataLoader implements CommandLineRunner {

    private final AuthorRepository authorRepository;
    private final ReaderRepository readerRepository;
    private final EmployeeRepository employeeRepository;
    private final GenreRepository genreRepository;
    private final FieldRepository fieldRepository;
    private final BookRepository bookRepository;
    private final MagazineRepository magazineRepository;
    private final HardCopyExemplarRepository hardCopyExemplarRepository;
    private final ElectronicCopyExemplarRepository electronicCopyExemplarRepository;
    private final ReservationRepository reservationRepository;
    private final HardCopyBorrowingRepository hardCopyBorrowingRepository;
    private final ElectronicCopyBorrowingRepository electronicCopyBorrowingRepository;
    private final FineRepository fineRepository;

    private final Faker faker;

    @Autowired
    public SampleDataLoader(
                            AuthorRepository authorRepository,
                            ReaderRepository readerRepository,
                            EmployeeRepository employeeRepository,
                            GenreRepository genreRepository,
                            FieldRepository fieldRepository,
                            BookRepository bookRepository,
                            MagazineRepository magazineRepository,
                            HardCopyExemplarRepository hardCopyExemplarRepository,
                            ElectronicCopyExemplarRepository electronicCopyExemplarRepository,
                            ReservationRepository reservationRepository,
                            HardCopyBorrowingRepository hardCopyBorrowingRepository,
                            ElectronicCopyBorrowingRepository electronicCopyBorrowingRepository,
                            FineRepository fineRepository
                            ) {
        this.authorRepository = authorRepository;
        this.readerRepository = readerRepository;
        this.employeeRepository = employeeRepository;
        this.genreRepository = genreRepository;
        this.fieldRepository = fieldRepository;
        this.bookRepository = bookRepository;
        this.magazineRepository = magazineRepository;
        this.hardCopyExemplarRepository = hardCopyExemplarRepository;
        this.electronicCopyExemplarRepository = electronicCopyExemplarRepository;
        this.reservationRepository = reservationRepository;
        this.hardCopyBorrowingRepository = hardCopyBorrowingRepository;
        this.electronicCopyBorrowingRepository = electronicCopyBorrowingRepository;
        this.fineRepository = fineRepository;

        this.faker = new Faker();
    }

    @Override
    public void run(String... args) throws Exception {
        if (authorRepository.count() == 0) this.generateAuthors();
        if (readerRepository.count() == 0) this.generateReaders();
        if (employeeRepository.count() == 0) this.generateEmployees();
        if (fieldRepository.count() == 0) this.generateFields();
        if (genreRepository.count() == 0) this.generateGenres();
        if (bookRepository.count() == 0) this.generateBooks();
        if (magazineRepository.count() == 0 ) this.generateMagazines();
        if (hardCopyExemplarRepository.count() == 0) this.generateHardCopies();
        if (electronicCopyExemplarRepository.count() == 0) this.generateElectronicCopies();
        if (reservationRepository.count() == 0) this.generateReservations();
        if (hardCopyBorrowingRepository.count() == 0) this.generateHardCopiesBorrowings();
        if (electronicCopyBorrowingRepository.count() == 0 ) this.generateElectronicCopiesBorrowings();
        if (fineRepository.count() == 0) this.generateFines();
    }

    private void generateAuthors() throws Exception{
        List<Author> authors = IntStream.rangeClosed(1,15).mapToObj(
                i-> new Author(
                        0L,
                        faker.name().firstName(),
                        faker.name().lastName(),
                        "http://localhost:8080/pis-library/api/v1/downloadFile/cartoon.jpg",
                        faker.date().birthday(20,80),
                        null,
                        null,
                        faker.lorem().paragraph(10),
                        null
                )
        ).collect(Collectors.toList());

        authorRepository.saveAll(authors);

        authors = IntStream.rangeClosed(1,15).mapToObj(
                i-> new Author(
                        0L,
                        faker.name().firstName(),
                        faker.name().lastName(),
                        "http://localhost:8080/pis-library/api/v1/downloadFile/cartoon.jpg",
                        faker.date().past(11000, TimeUnit.DAYS),
                        null,
                        null,
                        faker.lorem().paragraph(13),
                        faker.date().past(10, TimeUnit.DAYS)
                )
        ).collect(Collectors.toList());
        authorRepository.saveAll(authors);

    }

    private void generateReaders(){
        List<Reader> readers = IntStream.rangeClosed(1,30).mapToObj(
                i-> new Reader(
                        faker.internet().emailAddress(),
                        PasswordEncryptor.encrypt("reader123"),
                        faker.name().firstName(),
                        faker.name().lastName(),
                        faker.address().streetAddress(),
                        faker.address().buildingNumber(),
                        faker.address().city(),
                        faker.address().zipCode(),
                        RoleEnum.READER,
                        null,
                        null,
                        null
                )
        ).collect(Collectors.toList());
        readerRepository.saveAll(readers);
        Reader reader = new Reader(
                "reader@gmail.com",
                PasswordEncryptor.encrypt("reader123"),
                faker.name().firstName(),
                faker.name().lastName(),
                faker.address().streetAddress(),
                faker.address().buildingNumber(),
                faker.address().city(),
                faker.address().zipCode(),
                RoleEnum.READER,
                null,
                null,
                null);
        readerRepository.save(reader);
    }

    private void generateEmployees(){
        List<Employee> employees = IntStream.rangeClosed(1,10).mapToObj(
                i-> new Employee(
                        faker.internet().emailAddress(),
                        PasswordEncryptor.encrypt("employee123"),
                        faker.name().firstName(),
                        faker.name().lastName(),
                        faker.address().streetAddress(),
                        faker.address().buildingNumber(),
                        faker.address().city(),
                        faker.address().zipCode(),
                        RoleEnum.EMPLOYEE
                )
        ).collect(Collectors.toList());
        employeeRepository.saveAll(employees);
        employees = IntStream.rangeClosed(1,5).mapToObj(
                i-> new Employee(
                        faker.internet().emailAddress(),
                        PasswordEncryptor.encrypt("admin123"),
                        faker.name().firstName(),
                        faker.name().lastName(),
                        faker.address().streetAddress(),
                        faker.address().buildingNumber(),
                        faker.address().city(),
                        faker.address().zipCode(),
                        RoleEnum.ADMIN
                )
        ).collect(Collectors.toList());
        employeeRepository.saveAll(employees);
        Employee employee = new Employee(
                "employee@gmail.com",
                PasswordEncryptor.encrypt("employee123"),
                faker.name().firstName(),
                faker.name().lastName(),
                faker.address().streetAddress(),
                faker.address().buildingNumber(),
                faker.address().city(),
                faker.address().zipCode(),
                RoleEnum.EMPLOYEE
        );
        employeeRepository.save(employee);
        Employee admin = new Employee(
                "admin@gmail.com",
                PasswordEncryptor.encrypt("admin123"),
                faker.name().firstName(),
                faker.name().lastName(),
                faker.address().streetAddress(),
                faker.address().buildingNumber(),
                faker.address().city(),
                faker.address().zipCode(),
                RoleEnum.ADMIN
        );
        employeeRepository.save(admin);
    }

    private void generateGenres(){
        var genresStream = Stream.generate(()->faker.book().genre()).distinct(); // ziska unique hodnoty zanrov
        List<String> genresNames = genresStream.limit(25).collect(Collectors.toList());
        List<Genre> genres = IntStream.rangeClosed(1,25).mapToObj(
                i->new Genre(
                        genresNames.get(i-1)
                )
        ).collect(Collectors.toList());
        genreRepository.saveAll(genres);
    }

    private void generateFields(){
        var fieldsStream = Stream.generate(()->faker.book().genre()).distinct(); // ziska unique hodnoty zanrov
        List<String> fieldNames = fieldsStream.limit(15).collect(Collectors.toList());
        List<Field> fields = IntStream.rangeClosed(1,15).mapToObj(
                i->new Field(
                        fieldNames.get(i-1)
                )
        ).collect(Collectors.toList());
        fieldRepository.saveAll(fields);
    }

    private void generateBooks() throws Exception{
        Random rand = new Random();
        List<Book> books = IntStream.rangeClosed(1,50).mapToObj(
                i-> new Book(
                        faker.book().title(),
                        faker.lorem().paragraph(8 + rand.nextInt(5)),
                        faker.book().publisher(),
                        faker.nation().language(),
                        "http://localhost:8080/pis-library/api/v1/downloadFile/hp1.jpg",
                        faker.code().isbn13(),
                        faker.number().numberBetween(1, 5),
                        faker.date().birthday(0, 50),
                        faker.number().numberBetween(200, 558),
                        authorRepository.findById(new Random().nextLong(29)+1).stream().toList(),
                        genreRepository.findById(new Random().nextLong(24)+1).stream().toList(),
                        null,
                        null
                )).collect(Collectors.toList());
        bookRepository.saveAll(books);
    }

    private void generateMagazines() throws Exception{
        List<Magazine> magazines = IntStream.rangeClosed(1,60).mapToObj(
                i-> new Magazine(
                        faker.book().title(),
                        faker.lorem().paragraph(new Random().nextInt(7)+3),
                        faker.book().publisher(),
                        faker.nation().language(),
                        "http://localhost:8080/pis-library/api/v1/downloadFile/magazine.jpg",
                        faker.number().randomNumber(8, true),
                        faker.number().numberBetween(1,4),
                        faker.number().numberBetween(1,50), // rocnik
                        Arrays.asList(authorRepository.findById(new Random().nextLong(29)+1).get(),
                                authorRepository.findById(new Random().nextLong(29)+1).get()),
                        fieldRepository.findById(new Random().nextLong(14)+1).stream().toList(),
                        null,
                        null
                )
        ).collect(Collectors.toList());
        magazineRepository.saveAll(magazines);
    }

    private void generateHardCopies(){
        ExemplarEnum[] exemplarState = ExemplarEnum.values();
        Random rand = new Random();
        List<HardCopyExemplar> hardCopyExemplarsBooks = IntStream.rangeClosed(1,100).mapToObj(
                i->new HardCopyExemplar(
                        exemplarState[rand.nextInt(exemplarState.length-1)], // state
                        faker.bool().bool(),
                        faker.number().numberBetween(10,50),
                        faker.number().numberBetween(3,6),
                        bookRepository.findById(new Random().nextLong(49)+1).get(),
                        null,
                        null,
                        null
                )
        ).collect(Collectors.toList());
        hardCopyExemplarRepository.saveAll(hardCopyExemplarsBooks);

        List<HardCopyExemplar> hardCopyExemplarsMagazines = IntStream.rangeClosed(1,100).mapToObj(
                i->new HardCopyExemplar(
                        exemplarState[rand.nextInt(exemplarState.length)],
                        faker.bool().bool(),
                        faker.number().numberBetween(10,50),
                        faker.number().numberBetween(0,5),
                        null,
                        magazineRepository.findById(new Random().nextLong(59)+1).get(),
                        null,
                        null
                )
        ).collect(Collectors.toList());
        hardCopyExemplarRepository.saveAll(hardCopyExemplarsMagazines);
    }

    private void generateElectronicCopies() throws Exception{
        List<ElectronicCopyExemplar> electronicCopyExemplarsBooks = IntStream.rangeClosed(1,100).mapToObj(
                i->new ElectronicCopyExemplar(
                        ExemplarEnum.ELECTRONIC,
                        faker.bool().bool(),
                        faker.number().numberBetween(10,50),
                        faker.number().numberBetween(0,5),
                        bookRepository.findById(new Random().nextLong(49)+1).get(),
                        null,
                        null,
                        "http://localhost:8080/pis-library/api/v1/downloadFile/prednaska1.pdf"
                        )

        ).collect(Collectors.toList());
        electronicCopyExemplarRepository.saveAll(electronicCopyExemplarsBooks);

        List<ElectronicCopyExemplar> electronicCopyExemplarsMagazines = IntStream.rangeClosed(1,100).mapToObj(
                i->new ElectronicCopyExemplar(
                        ExemplarEnum.ELECTRONIC,
                        faker.bool().bool(),
                        faker.number().numberBetween(10,50),
                        faker.number().numberBetween(0,5),
                        null,
                        magazineRepository.findById(new Random().nextLong(59)+1).get(),
                        null,
                        "http://localhost:8080/pis-library/api/v1/downloadFile/prednaska1.pdf"
                )
        ).collect(Collectors.toList());
        electronicCopyExemplarRepository.saveAll(electronicCopyExemplarsMagazines);
    }

    private void generateReservations(){
        // minulost
        List<Reservation> reservations = IntStream.rangeClosed(0,40).mapToObj(
                i->new Reservation(
                        faker.date().past(150,TimeUnit.DAYS),
                        faker.date().past(50,TimeUnit.DAYS),
                        ReservationEnum.NOT_ACTIVE,
                        readerRepository.findById(new Random().nextLong(19)+1).get(),
                        hardCopyExemplarRepository.findById(new Random().nextLong(19)+1).get()
                )
        ).collect(Collectors.toList());
        reservationRepository.saveAll(reservations);

        reservations = IntStream.rangeClosed(0,40).mapToObj(
                i->new Reservation(
                        faker.date().past(50,TimeUnit.DAYS),
                        faker.date().future(50,TimeUnit.DAYS),
                        ReservationEnum.ACTIVE,
                        readerRepository.findById(new Random().nextLong(29)+1).get(),
                        hardCopyExemplarRepository.findById(new Random().nextLong(24)+21).get()
                )
        ).collect(Collectors.toList());
        reservationRepository.saveAll(reservations);
    }

    private void generateHardCopiesBorrowings() {

        // z tychto budu pokuty nezaplatene
        List<HardCopyBorrowing> hardCopyBorrowings = IntStream.rangeClosed(1, 20).mapToObj(
                i->new HardCopyBorrowing(
                        faker.date().past(100, TimeUnit.DAYS),
                        faker.date().past(10, TimeUnit.DAYS),
                        faker.number().numberBetween(2,4),
                        readerRepository.findById(new Random().nextLong(29)+1).get(),
                        HardCopyBorrowingEnum.TO_RETURN,
                        null,
                        null,
                        hardCopyExemplarRepository.findById(new Random().nextLong(19)+1).get()

                )
        ).collect(Collectors.toList());
        hardCopyBorrowingRepository.saveAll(hardCopyBorrowings);

        hardCopyBorrowings = IntStream.rangeClosed(1, 20).mapToObj(
                i->new HardCopyBorrowing(
                        faker.date().past(100, TimeUnit.DAYS),
                        faker.date().past(10, TimeUnit.DAYS),
                        faker.number().numberBetween(2,4),
                        readerRepository.findById(new Random().nextLong(29)+1).get(),
                        HardCopyBorrowingEnum.RETURNED,
                        faker.date().past(5, TimeUnit.DAYS),
                        null,
                        hardCopyExemplarRepository.findById(new Random().nextLong(19)+21).get()

                )
        ).collect(Collectors.toList());
        hardCopyBorrowingRepository.saveAll(hardCopyBorrowings);

        List<HardCopyBorrowing> hardCopyBorrowingsActual = IntStream.rangeClosed(1, 25).mapToObj(
                i->new HardCopyBorrowing(
                        faker.date().past(100, TimeUnit.DAYS),
                        faker.date().future(30, TimeUnit.DAYS),
                        faker.number().numberBetween(2,4),
                        readerRepository.findById(new Random().nextLong(29)+1).get(),
                        HardCopyBorrowingEnum.CAN_NOT_PROLONG,
                        null,
                        null,
                        hardCopyExemplarRepository.findById(new Random().nextLong(24)+41).get()
                )
        ).collect(Collectors.toList());
        hardCopyBorrowingRepository.saveAll(hardCopyBorrowingsActual);

        hardCopyBorrowings = IntStream.rangeClosed(1, 40).mapToObj(
                i->new HardCopyBorrowing(
                        faker.date().past(100, TimeUnit.DAYS),
                        faker.date().future(20, TimeUnit.DAYS),
                        faker.number().numberBetween(0,2),
                        readerRepository.findById(new Random().nextLong(29)+1).get(),
                        HardCopyBorrowingEnum.ACTIVE,
                        null,
                        null,
                        hardCopyExemplarRepository.findById(new Random().nextLong(69)+46).get()
                )
        ).collect(Collectors.toList());
        hardCopyBorrowingRepository.saveAll(hardCopyBorrowings);
    }

    private void generateElectronicCopiesBorrowings(){
        List<ElectronicCopyBorrowing> electronicCopyBorrowings = new ArrayList<>();
        for (int i = 1; i < 43; i++) {
            ElectronicCopyExemplar exemplar = electronicCopyExemplarRepository.findById(new Random().nextLong(199)+1).get();
            ElectronicCopyBorrowing electronicCopyBorrowing;
            if (exemplar.getMaximumNumberOfExtension() > 0){
                electronicCopyBorrowing = new ElectronicCopyBorrowing(
                        faker.date().past(50, TimeUnit.DAYS),
                        faker.date().future(5, TimeUnit.DAYS),
                        faker.number().numberBetween(1, exemplar.getMaximumNumberOfExtension()),
                        readerRepository.findById(new Random().nextLong(29)+1).get(),
                        exemplar
                );
            }
            else {
                electronicCopyBorrowing = new ElectronicCopyBorrowing(
                        faker.date().past(50, TimeUnit.DAYS),
                        faker.date().future(5, TimeUnit.DAYS),
                        0,
                        readerRepository.findById(new Random().nextLong(29)+1).get(),
                        exemplar
                );
            }


            electronicCopyBorrowings.add(electronicCopyBorrowing);
        }
        electronicCopyBorrowingRepository.saveAll(electronicCopyBorrowings);
    }

    private void generateFines(){
        List<Fine> fines = IntStream.rangeClosed(1,20).mapToObj(
                i->new Fine(
                        faker.number().numberBetween(1,100),
                        FineEnum.UNPAID,
                        hardCopyBorrowingRepository.findById(new Random().nextLong(19)+1).get()
                )
        ).collect(Collectors.toList());
        fineRepository.saveAll(fines);

        fines = IntStream.rangeClosed(1,20).mapToObj(
                i->new Fine(
                        faker.number().numberBetween(1,100),
                        FineEnum.PAID,
                        hardCopyBorrowingRepository.findById(new Random().nextLong(19)+21).get()
                )
        ).collect(Collectors.toList());
        fineRepository.saveAll(fines);
    }

}
