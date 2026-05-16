namespace CartService.Application.Exceptions;

public class ItemDoesNotExistException(string message) : Exception(message);