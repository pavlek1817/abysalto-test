namespace CartService.Application.Exceptions;

public class NotEnoughQuantityException(string message) : Exception(message);
