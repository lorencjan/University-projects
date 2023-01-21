package com.fit.vut.Library.services;

import com.fit.vut.Library.dtos.BookDto;
import com.fit.vut.Library.entities.Book;
import com.fit.vut.Library.repositories.BookRepository;
import com.fit.vut.Library.services.filters.BookFilter;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import javax.persistence.EntityManager;
import javax.persistence.PersistenceContext;
import javax.persistence.criteria.*;
import java.util.*;

@Service
@Transactional
public class BookService {

    @PersistenceContext
    private EntityManager em;
    private final BookRepository repo;

    public BookService(BookRepository repo) {
        this.repo = repo;
    }

    public void save (BookDto book) { repo.save(book.toEntity()); }

    public void delete (Long id){ repo.delete(new Book(id)); }

    public BookDto getById (Long id){
        Optional<Book> book = repo.findById(id);
        return book.isEmpty() ? null : book.get().toDto();
    }

    public List<BookDto> get(BookFilter filter) {
        if (filter == null)
            return repo.findAll().stream().map(x -> x.toDto()).toList();

        CriteriaBuilder cb = em.getCriteriaBuilder();
        CriteriaQuery<Book> cq = cb.createQuery(Book.class);
        Root<Book> quest = cq.from(Book.class);
        List<Predicate> predicates = new ArrayList();

        if (filter.getName() != null)
            predicates.add(cb.like(quest.get("name"), "%" + filter.getName() + "%"));

        if (filter.getPublisher() != null)
            predicates.add(cb.like(quest.get("publisher"), "%" + filter.getPublisher() + "%"));

        if (filter.getLanguage() != null)
            predicates.add(cb.like(quest.get("language"), "%" + filter.getLanguage() + "%"));

        if (filter.getIsbn() != null)
            predicates.add(cb.like(quest.get("isbn"), "%" + filter.getIsbn() + "%"));

        cq.select(quest).where(predicates.toArray(new Predicate[] {}));
        List<Book> books = em.createQuery(cq).getResultList();

        if (filter.getAuthor() != null) {
            books = books.stream()
                    .filter(b -> b.getAuthors().stream()
                            .anyMatch(a -> a.getName().contains(filter.getAuthor())
                                    || a.getSurname().contains(filter.getAuthor()))).toList();
        }

        if (filter.getGenres() != null && !filter.getGenres().isEmpty())
            books = books.stream().filter(b -> b.getGenres().stream().anyMatch(g -> filter.getGenres().contains(g.getName()))).toList();

        return books.stream().map(x -> x.toDto()).toList();
    }
}
